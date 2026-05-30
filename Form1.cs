using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TemperatureAnalyzer
{
    public partial class Form1 : Form
    {
        private List<TemperatureData> _data;          // загруженные данные
        private MovingAverageForecaster _forecaster;  // объект для прогноза
        private List<double> _forecastValues;         // рассчитанные прогнозные значения

        public Form1()
        {
            InitializeComponent();
            _forecaster = new MovingAverageForecaster();
            _forecastValues = new List<double>();
            HookEvents();
        }

        private void HookEvents()
        {
            открытьToolStripMenuItem.Click += OpenFile_Click;
            экспортToolStripMenuItem.Click += ExportGraph_Click;
            btnForecast.Click += BtnForecast_Click;
            btnApplyRange.Click += BtnApplyRange_Click;
            trackStart.ValueChanged += TrackRange_ValueChanged;
            trackEnd.ValueChanged += TrackRange_ValueChanged;
            graphPanel.Resize += (s, e) => graphPanel.Invalidate();
        }

        private void OpenFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV файлы (*.csv)|*.csv|Все файлы (*.*)|*.*";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    _data = TemperatureData.LoadFromCsv(openFileDialog1.FileName);
                    if (_data.Count == 0) throw new Exception("Файл не содержит данных");
                    LoadDataToGrid();
                    CalculateTemperatureRanges();
                    _forecastValues.Clear();
                    graphPanel.Invalidate();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
                }
            }
        }

        private void LoadDataToGrid()
        {
            dataGridView1.DataSource = null;
            var displayData = _data.Select(d => new
            {
                d.Day,
                d.Date,
                d.MaxTemp,
                d.MinTemp,
                d.AvgTemp,
                d.Description,
                Перепад = d.TempRange
            }).ToList();
            dataGridView1.DataSource = displayData;
            dataGridView1.AutoResizeColumns();
        }

        private void CalculateTemperatureRanges()
        {
            if (_data == null || _data.Count == 0) return;
            var maxRangeDay = _data.OrderByDescending(d => d.TempRange).First();
            var minRangeDay = _data.OrderBy(d => d.TempRange).First();
            labelMaxDiff.Text = $"Макс. перепад: день {maxRangeDay.Day} ({maxRangeDay.TempRange:F1}°C)";
            labelMinDiff.Text = $"Мин. перепад: день {minRangeDay.Day} ({minRangeDay.TempRange:F1}°C)";
        }

        private void BtnForecast_Click(object sender, EventArgs e)
        {
            if (_data == null || _data.Count == 0)
            {
                MessageBox.Show("Сначала загрузите файл с данными.", "Предупреждение");
                return;
            }
            int windowSize = (int)numericWindow.Value;
            int forecastDays = (int)numericForecastDays.Value;
            var historicalValues = _data.Select(d => d.AvgTemp).ToList();
            try
            {
                _forecastValues = _forecaster.Forecast(historicalValues, windowSize, forecastDays);
                string forecastText = "Прогноз средней температуры (скользящая средняя):\n";
                for (int i = 0; i < _forecastValues.Count; i++)
                {
                    forecastText += $"День {_data.Last().Day + i + 1}: {_forecastValues[i]:F1}°C\n";
                }
                MessageBox.Show(forecastText, "Результат прогноза");
                graphPanel.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка прогнозирования: {ex.Message}", "Ошибка");
            }
        }

        private void BtnApplyRange_Click(object sender, EventArgs e)
        {
            graphPanel.Invalidate();
        }

        private void TrackRange_ValueChanged(object sender, EventArgs e)
        {
            labelRange.Text = $"Дни с {trackStart.Value} по {trackEnd.Value}";
        }

        private void ExportGraph_Click(object sender, EventArgs e)
        {
            if (graphPanel.Width == 0 || graphPanel.Height == 0) return;
            using (Bitmap bmp = new Bitmap(graphPanel.Width, graphPanel.Height))
            {
                graphPanel.DrawToBitmap(bmp, graphPanel.ClientRectangle);
                saveFileDialog1.Filter = "PNG файл (*.png)|*.png|JPEG (*.jpg)|*.jpg";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    bmp.Save(saveFileDialog1.FileName);
                    MessageBox.Show("График сохранён.", "Успех");
                }
            }
        }

        // Отрисовка графика
        private void graphPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_data == null || _data.Count == 0)
            {
                e.Graphics.DrawString("Загрузите CSV-файл", this.Font, Brushes.Black, 10, 10);
                return;
            }

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            int width = graphPanel.Width;
            int height = graphPanel.Height;
            if (width <= 20 || height <= 20) return;

            // Отступы для осей и подписей
            int marginLeft = 60;
            int marginRight = 40;
            int marginTop = 30;
            int marginBottom = 50;

            int graphWidth = width - marginLeft - marginRight;
            int graphHeight = height - marginTop - marginBottom;
            if (graphWidth <= 0 || graphHeight <= 0) return;

            // Рисуем оси
            using (Pen axisPen = new Pen(Color.Black, 1))
            {
                g.DrawLine(axisPen, marginLeft, marginTop, marginLeft, marginTop + graphHeight);
                g.DrawLine(axisPen, marginLeft, marginTop + graphHeight, marginLeft + graphWidth, marginTop + graphHeight);
            }

            // Подписи осей
            using (Font axisFont = new Font(this.Font.FontFamily, 9, FontStyle.Bold))
            {
                g.DrawString("День", axisFont, Brushes.Black, marginLeft + graphWidth / 2 - 15, marginTop + graphHeight + 25);
                DrawRotatedText(g, "Температура (°C)", axisFont, Brushes.Black, 18, marginTop + graphHeight / 2, -90);
            }

            int startDay = trackStart.Value;
            int endDay = trackEnd.Value;
            if (startDay < 1) startDay = 1;
            if (endDay > _data.Count) endDay = _data.Count;
            if (startDay > endDay) startDay = endDay;

            var filteredData = _data.Where(d => d.Day >= startDay && d.Day <= endDay).ToList();
            if (filteredData.Count == 0) return;

            // Определяем диапазон температур для масштабирования
            double minTemp = filteredData.Min(d => Math.Min(d.MinTemp, d.MaxTemp));
            double maxTemp = filteredData.Max(d => Math.Max(d.MaxTemp, d.MinTemp));
            if (_forecastValues != null && _forecastValues.Count > 0)
            {
                minTemp = Math.Min(minTemp, _forecastValues.Min());
                maxTemp = Math.Max(maxTemp, _forecastValues.Max());
            }
            if (Math.Abs(maxTemp - minTemp) < 0.1)
            {
                minTemp -= 5;
                maxTemp += 5;
            }
            double rangeY = maxTemp - minTemp;

            // Функции преобразования координат
            Func<int, float> dayToX = (day) =>
            {
                float x = marginLeft + (float)((day - startDay) * graphWidth / (double)(endDay - startDay));
                return Math.Min(marginLeft + graphWidth, Math.Max(marginLeft, x));
            };
            Func<double, float> tempToY = (temp) =>
            {
                float y = marginTop + graphHeight - (float)((temp - minTemp) * graphHeight / rangeY);
                return Math.Min(marginTop + graphHeight, Math.Max(marginTop, y));
            };

            // Линии сетки и подписи по Y
            using (Pen gridPen = new Pen(Color.LightGray, 1))
            using (Font tickFont = new Font(this.Font.FontFamily, 8))
            {
                int ySteps = Math.Min(8, (int)Math.Ceiling(rangeY / 2));
                if (ySteps < 3) ySteps = 3;
                for (int i = 0; i <= ySteps; i++)
                {
                    double temp = minTemp + i * rangeY / ySteps;
                    float y = tempToY(temp);
                    g.DrawLine(gridPen, marginLeft, y, marginLeft + graphWidth, y);
                    string label = temp.ToString("F1");
                    SizeF labelSize = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, Brushes.Black, marginLeft - labelSize.Width - 4, y - labelSize.Height / 2);
                }
            }

            // Подписи по X (автоматический шаг для предотвращения налезания)
            using (Font tickFont = new Font(this.Font.FontFamily, 8))
            {
                int totalDays = endDay - startDay + 1;
                int step = 1;
                if (totalDays > 20) step = 2;
                if (totalDays > 30) step = 3;
                if (totalDays > 40) step = 5;
                for (int day = startDay; day <= endDay; day += step)
                {
                    float x = dayToX(day);
                    g.DrawLine(Pens.LightGray, x, marginTop + graphHeight, x, marginTop + graphHeight + 5);
                    string label = day.ToString();
                    SizeF labelSize = g.MeasureString(label, tickFont);
                    g.DrawString(label, tickFont, Brushes.Black, x - labelSize.Width / 2, marginTop + graphHeight + 6);
                }
            }

            // График максимальной температуры (красный)
            using (Pen redPen = new Pen(Color.Red, 2))
            {
                var points = filteredData.Select(d => new PointF(dayToX(d.Day), tempToY(d.MaxTemp))).ToArray();
                if (points.Length > 1) g.DrawLines(redPen, points);
            }

            // График минимальной температуры (синий)
            using (Pen bluePen = new Pen(Color.Blue, 2))
            {
                var points = filteredData.Select(d => new PointF(dayToX(d.Day), tempToY(d.MinTemp))).ToArray();
                if (points.Length > 1) g.DrawLines(bluePen, points);
            }

            // Прогноз (зелёный пунктир)
            if (_forecastValues != null && _forecastValues.Count > 0)
            {
                using (Pen greenPen = new Pen(Color.Green, 2) { DashStyle = DashStyle.Dash })
                {
                    int lastDay = _data.Last().Day;
                    var forecastPoints = new List<PointF>();
                    for (int i = 0; i < _forecastValues.Count; i++)
                    {
                        int forecastDay = lastDay + i + 1;
                        if (forecastDay >= startDay && forecastDay <= endDay)
                            forecastPoints.Add(new PointF(dayToX(forecastDay), tempToY(_forecastValues[i])));
                    }
                    if (forecastPoints.Count > 1)
                        g.DrawLines(greenPen, forecastPoints.ToArray());
                }
            }

            // Легенда (в правом верхнем углу с фоном)
            using (Font legendFont = new Font(this.Font.FontFamily, 8, FontStyle.Bold))
            using (Brush bgBrush = new SolidBrush(Color.FromArgb(200, Color.White)))
            {
                int legendX = width - 130;
                int legendY = marginTop;
                int legendW = 120;
                int legendH = 55;
                g.FillRectangle(bgBrush, legendX, legendY, legendW, legendH);
                g.DrawRectangle(Pens.Black, legendX, legendY, legendW, legendH);
                g.DrawString("Макс.", legendFont, Brushes.Red, legendX + 5, legendY + 5);
                g.DrawString("Мин.", legendFont, Brushes.Blue, legendX + 5, legendY + 25);
                if (_forecastValues != null && _forecastValues.Count > 0)
                    g.DrawString("Прогноз (ср.)", legendFont, Brushes.Green, legendX + 5, legendY + 45);
            }
        }

        /// <summary>
        /// Рисование текста под углом.
        /// </summary>
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