using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using TemperatureAnalyzer;
namespace TemperatureAnalyzer
{
    public partial class RunningForm : Form
    {
        private List<RunningData> _data;
        private MovingAverageForecaster _forecaster;
        private List<double> _forecastValues;

        // Контролы
        private DataGridView dataGridView;
        private Button btnLoad, btnForecast, btnExport;
        private TrackBar trackStart, trackEnd;
        private NumericUpDown numericWindow, numericForecastDays;
        private Label labelWeekendTotal, labelRange, labelWindow, labelDays;
        private Panel graphPanel;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;

        public RunningForm()
        {
            _forecaster = new MovingAverageForecaster();
            _forecastValues = new List<double>();
            InitializeComponents();
            HookEvents();
        }

        private void InitializeComponents()
        {
            this.Text = "Вариант 1: Анализ пробежек";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // Кнопка загрузки
            btnLoad = new Button
            {
                Text = "📂 Загрузить CSV",
                Location = new Point(20, 20),
                Size = new Size(120, 35),
                BackColor = Color.LightBlue,
                FlatStyle = FlatStyle.Flat
            };

            // Метка суммы за выходные
            labelWeekendTotal = new Label
            {
                Text = "Сумма км за выходные: -",
                Location = new Point(160, 25),
                Size = new Size(250, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            // Таблица
            dataGridView = new DataGridView
            {
                Location = new Point(20, 70),
                Size = new Size(500, 300),
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
            };

            // Панель для графика
            graphPanel = new Panel
            {
                Location = new Point(540, 70),
                Size = new Size(530, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            // Label для периода
            labelRange = new Label
            {
                Text = "Диапазон дней: 1 - 30",
                Location = new Point(20, 390),
                Size = new Size(200, 25)
            };

            // TrackBar начала
            trackStart = new TrackBar
            {
                Location = new Point(20, 420),
                Size = new Size(200, 45),
                Minimum = 1,
                Maximum = 31,
                Value = 1,
                TickFrequency = 5
            };

            // TrackBar конца
            trackEnd = new TrackBar
            {
                Location = new Point(230, 420),
                Size = new Size(200, 45),
                Minimum = 1,
                Maximum = 31,
                Value = 31,
                TickFrequency = 5
            };

            // Метка размера окна скользящей средней
            labelWindow = new Label
            {
                Text = "Окно (n):",
                Location = new Point(20, 480),
                Size = new Size(60, 25)
            };

            numericWindow = new NumericUpDown
            {
                Location = new Point(90, 478),
                Size = new Size(60, 25),
                Minimum = 2,
                Maximum = 10,
                Value = 3
            };

            // Метка количества дней прогноза
            labelDays = new Label
            {
                Text = "Дней прогноза:",
                Location = new Point(170, 480),
                Size = new Size(80, 25)
            };

            numericForecastDays = new NumericUpDown
            {
                Location = new Point(260, 478),
                Size = new Size(60, 25),
                Minimum = 1,
                Maximum = 15,
                Value = 5
            };

            // Кнопка прогноза
            btnForecast = new Button
            {
                Text = "📈 Прогноз",
                Location = new Point(340, 475),
                Size = new Size(100, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat
            };

            // Кнопка экспорта
            btnExport = new Button
            {
                Text = "💾 Экспорт графика",
                Location = new Point(460, 475),
                Size = new Size(120, 35),
                BackColor = Color.LightGray,
                FlatStyle = FlatStyle.Flat
            };

            // Диалоги
            openFileDialog = new OpenFileDialog
            {
                Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*"
            };
            saveFileDialog = new SaveFileDialog
            {
                Filter = "PNG файл (*.png)|*.png|JPEG файл (*.jpg)|*.jpg"
            };

            // Добавляем контролы на форму
            this.Controls.AddRange(new Control[] {
                btnLoad, labelWeekendTotal, dataGridView, graphPanel,
                labelRange, trackStart, trackEnd,
                labelWindow, numericWindow, labelDays, numericForecastDays,
                btnForecast, btnExport
            });
        }

        private void HookEvents()
        {
            btnLoad.Click += BtnLoad_Click;
            btnForecast.Click += BtnForecast_Click;
            btnExport.Click += BtnExport_Click;
            trackStart.ValueChanged += TrackRange_ValueChanged;
            trackEnd.ValueChanged += TrackRange_ValueChanged;
            graphPanel.Paint += GraphPanel_Paint;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _data = RunningData.LoadFromCsv(openFileDialog.FileName);
                    if (_data.Count == 0) throw new Exception("Файл не содержит данных");

                    LoadDataToGrid();

                    double weekendTotal = RunningData.GetWeekendTotalKm(_data);
                    labelWeekendTotal.Text = $"🏃 Сумма км за выходные: {weekendTotal:F2} км";

                    trackStart.Maximum = _data.Count;
                    trackEnd.Maximum = _data.Count;
                    trackEnd.Value = _data.Count;

                    _forecastValues.Clear();
                    graphPanel.Invalidate();

                    MessageBox.Show($"Загружено {_data.Count} дней", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadDataToGrid()
        {
            dataGridView.DataSource = null;
            var displayData = _data.Select(d => new
            {
                d.Day,
                Дата = d.Date.ToString("yyyy-MM-dd"),
                Дистанция_км = d.DistanceKm,
                Длительность_мин = d.DurationMinutes,
                Средняя_скорость = d.AvgSpeedKmh,
                Макс_скорость = d.MaxSpeedKmh,
                Мин_скорость = d.MinSpeedKmh,
                Пульс = d.AvgPulse
            }).ToList();
            dataGridView.DataSource = displayData;
        }

        private void BtnForecast_Click(object sender, EventArgs e)
        {
            if (_data == null || _data.Count == 0)
            {
                MessageBox.Show("Сначала загрузите файл с данными", "Предупреждение",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int windowSize = (int)numericWindow.Value;
            int forecastDays = (int)numericForecastDays.Value;
            var historicalValues = _data.Select(d => d.DistanceKm).ToList();

            try
            {
                _forecastValues = _forecaster.Forecast(historicalValues, windowSize, forecastDays);

                string forecastText = "📊 Прогноз дистанции (скользящая средняя):\n\n";
                for (int i = 0; i < _forecastValues.Count; i++)
                {
                    forecastText += $"День {_data.Last().Day + i + 1}: {_forecastValues[i]:F2} км\n";
                }

                MessageBox.Show(forecastText, "Результат прогноза",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                graphPanel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка прогнозирования: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (graphPanel.Width == 0 || graphPanel.Height == 0) return;

            using (Bitmap bmp = new Bitmap(graphPanel.Width, graphPanel.Height))
            {
                graphPanel.DrawToBitmap(bmp, graphPanel.ClientRectangle);
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog.FileName);
                    MessageBox.Show("График сохранён!", "Успех",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void TrackRange_ValueChanged(object sender, EventArgs e)
        {
            if (trackStart.Value > trackEnd.Value)
            {
                if (sender == trackStart)
                    trackEnd.Value = trackStart.Value;
                else
                    trackStart.Value = trackEnd.Value;
            }
            labelRange.Text = $"Диапазон дней: {trackStart.Value} - {trackEnd.Value}";
            graphPanel.Invalidate();
        }

        private void GraphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_data == null || _data.Count == 0)
            {
                e.Graphics.DrawString("Загрузите CSV-файл с данными о пробежках",
                    this.Font, Brushes.Gray, 10, 10);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int width = graphPanel.Width;
            int height = graphPanel.Height;
            if (width <= 20 || height <= 20) return;

            int marginLeft = 60, marginRight = 40, marginTop = 30, marginBottom = 50;
            int graphWidth = width - marginLeft - marginRight;
            int graphHeight = height - marginTop - marginBottom;

            if (graphWidth <= 0 || graphHeight <= 0) return;

            // Рисуем оси
            using (Pen axisPen = new Pen(Color.Black, 2))
            {
                g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, marginTop + graphHeight);
                g.DrawLine(axisPen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight);
            }

            // Подписи осей
            using (Font axisFont = new Font("Segoe UI", 10, FontStyle.Bold))
            {
                g.DrawString("День", axisFont, Brushes.Black, marginLeft + graphWidth / 2 - 15, marginTop + graphHeight + 30);
                DrawRotatedText(g, "Дистанция (км)", axisFont, Brushes.Black, 18, marginTop + graphHeight / 2, -90);
            }

            int startDay = trackStart.Value;
            int endDay = trackEnd.Value;
            var filteredData = _data.Where(d => d.Day >= startDay && d.Day <= endDay).ToList();
            if (filteredData.Count == 0) return;

            double minDist = filteredData.Min(d => d.DistanceKm);
            double maxDist = filteredData.Max(d => d.DistanceKm);

            if (_forecastValues != null && _forecastValues.Count > 0)
            {
                minDist = Math.Min(minDist, _forecastValues.Min());
                maxDist = Math.Max(maxDist, _forecastValues.Max());
            }
            if (Math.Abs(maxDist - minDist) < 0.1)
            {
                minDist = 0;
                maxDist = maxDist + 5;
            }
            double rangeY = maxDist - minDist;

            Func<int, float> dayToX = (day) =>
            {
                return marginLeft + (float)((day - startDay) * graphWidth / (double)(endDay - startDay));
            };
            Func<double, float> distToY = (dist) =>
            {
                return marginTop + graphHeight - (float)((dist - minDist) * graphHeight / rangeY);
            };

            // Сетка Y
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            using (Font tickFont = new Font("Segoe UI", 8))
            {
                int ySteps = 6;
                for (int i = 0; i <= ySteps; i++)
                {
                    double dist = minDist + i * rangeY / ySteps;
                    float y = distToY(dist);
                    g.DrawLine(gridPen, marginLeft, y, marginLeft + graphWidth, y);
                    string label = dist.ToString("F1");
                    SizeF labelSize = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, Brushes.Black, marginLeft - labelSize.Width - 4, y - labelSize.Height / 2);
                }
            }

            // Сетка X
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            using (Font tickFont = new Font("Segoe UI", 8))
            {
                int totalDays = endDay - startDay + 1;
                int step = totalDays > 30 ? 5 : totalDays > 15 ? 3 : 2;
                for (int day = startDay; day <= endDay; day += step)
                {
                    float x = dayToX(day);
                    g.DrawLine(gridPen, x, marginTop + graphHeight, x, marginTop + graphHeight + 5);
                    g.DrawString(day.ToString(), tickFont, Brushes.Black, x - 8, marginTop + graphHeight + 8);
                }
            }

            // График дистанции (синий)
            using (Pen bluePen = new Pen(Color.Blue, 3))
            {
                var points = filteredData.Select(d => new PointF(dayToX(d.Day), distToY(d.DistanceKm))).ToArray();
                if (points.Length > 1) g.DrawLines(bluePen, points);

                // Точки
                foreach (var p in points)
                {
                    g.FillEllipse(Brushes.Blue, p.X - 3, p.Y - 3, 6, 6);
                }
            }

            // Прогноз (зелёный пунктир)
            if (_forecastValues != null && _forecastValues.Count > 0)
            {
                using (Pen greenPen = new Pen(Color.Green, 2.5f) { DashStyle = DashStyle.Dash })
                {
                    int lastDay = _data.Last().Day;
                    var forecastPoints = new List<PointF>();
                    for (int i = 0; i < _forecastValues.Count; i++)
                    {
                        int forecastDay = lastDay + i + 1;
                        if (forecastDay >= startDay && forecastDay <= endDay)
                        {
                            forecastPoints.Add(new PointF(dayToX(forecastDay), distToY(_forecastValues[i])));
                        }
                    }
                    if (forecastPoints.Count > 1)
                        g.DrawLines(greenPen, forecastPoints.ToArray());
                }
            }

            // Легенда
            using (Font legendFont = new Font("Segoe UI", 8, FontStyle.Bold))
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(220, Color.White)))
            {
                int legendX = width - 110;
                int legendY = marginTop;
                int legendW = 100;
                int legendH = 40;
                g.FillRectangle(bgBrush, legendX, legendY, legendW, legendH);
                g.DrawRectangle(Pens.Black, legendX, legendY, legendW, legendH);
                g.DrawString("Дистанция", legendFont, Brushes.Blue, legendX + 5, legendY + 5);
                if (_forecastValues != null && _forecastValues.Count > 0)
                    g.DrawString("Прогноз", legendFont, Brushes.Green, legendX + 5, legendY + 22);
            }
        }

        private void DrawRotatedText(Graphics g, string text, Font font, Brush brush, float x, float y, float angle)
        {
            var state = g.Save();
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.DrawString(text, font, brush, 0, 0);
            g.Restore(state);
        }
    }
}