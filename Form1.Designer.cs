namespace TemperatureAnalyzer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.экспортToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBoxForecast = new System.Windows.Forms.GroupBox();
            this.btnForecast = new System.Windows.Forms.Button();
            this.labelDays = new System.Windows.Forms.Label();
            this.labelWindow = new System.Windows.Forms.Label();
            this.numericForecastDays = new System.Windows.Forms.NumericUpDown();
            this.numericWindow = new System.Windows.Forms.NumericUpDown();
            this.groupBoxRange = new System.Windows.Forms.GroupBox();
            this.btnApplyRange = new System.Windows.Forms.Button();
            this.labelRange = new System.Windows.Forms.Label();
            this.trackEnd = new System.Windows.Forms.TrackBar();
            this.trackStart = new System.Windows.Forms.TrackBar();
            this.groupBoxDiff = new System.Windows.Forms.GroupBox();
            this.labelMaxDiff = new System.Windows.Forms.Label();
            this.labelMinDiff = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.graphPanel = new System.Windows.Forms.Panel(); // <-- вместо Chart

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBoxForecast.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericForecastDays)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWindow)).BeginInit();
            this.groupBoxRange.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackStart)).BeginInit();
            this.groupBoxDiff.SuspendLayout();
            this.SuspendLayout();

            // dataGridView1
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dataGridView1.Location = new System.Drawing.Point(0, 28);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(984, 150);
            this.dataGridView1.TabIndex = 0;

            // menuStrip1
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.открытьToolStripMenuItem, this.экспортToolStripMenuItem });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(984, 28);
            this.menuStrip1.TabIndex = 2;
            this.открытьToolStripMenuItem.Text = "Открыть";
            this.экспортToolStripMenuItem.Text = "Экспорт графика";

            // graphPanel (новый элемент)
            this.graphPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.graphPanel.Location = new System.Drawing.Point(0, 178);
            this.graphPanel.Name = "graphPanel";
            this.graphPanel.Size = new System.Drawing.Size(984, 362);
            this.graphPanel.TabIndex = 1;
            this.graphPanel.BackColor = System.Drawing.Color.White;
            this.graphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.graphPanel_Paint);

            // groupBoxForecast
            this.groupBoxForecast.Controls.Add(this.btnForecast);
            this.groupBoxForecast.Controls.Add(this.labelDays);
            this.groupBoxForecast.Controls.Add(this.labelWindow);
            this.groupBoxForecast.Controls.Add(this.numericForecastDays);
            this.groupBoxForecast.Controls.Add(this.numericWindow);
            this.groupBoxForecast.Location = new System.Drawing.Point(12, 540);
            this.groupBoxForecast.Size = new System.Drawing.Size(300, 100);
            this.groupBoxForecast.Text = "Прогнозирование";
            this.btnForecast.Location = new System.Drawing.Point(216, 22);
            this.btnForecast.Size = new System.Drawing.Size(75, 50);
            this.btnForecast.Text = "Прогноз";
            this.labelDays.Text = "Прогноз на дней:";
            this.labelWindow.Text = "Период сглаживания:";
            this.numericForecastDays.Minimum = 1; this.numericForecastDays.Value = 5;
            this.numericWindow.Minimum = 2; this.numericWindow.Value = 5;

            // groupBoxRange
            this.groupBoxRange.Controls.Add(this.btnApplyRange);
            this.groupBoxRange.Controls.Add(this.labelRange);
            this.groupBoxRange.Controls.Add(this.trackEnd);
            this.groupBoxRange.Controls.Add(this.trackStart);
            this.groupBoxRange.Location = new System.Drawing.Point(330, 540);
            this.groupBoxRange.Size = new System.Drawing.Size(300, 100);
            this.groupBoxRange.Text = "Выбор периода (дни)";
            this.btnApplyRange.Location = new System.Drawing.Point(210, 20);
            this.btnApplyRange.Size = new System.Drawing.Size(75, 30);
            this.btnApplyRange.Text = "Применить";
            this.labelRange.Text = "Дни с 1 по 31";
            this.trackEnd.Maximum = 31; this.trackEnd.Minimum = 1; this.trackEnd.Value = 31;
            this.trackStart.Maximum = 31; this.trackStart.Minimum = 1; this.trackStart.Value = 1;

            // groupBoxDiff
            this.groupBoxDiff.Controls.Add(this.labelMaxDiff);
            this.groupBoxDiff.Controls.Add(this.labelMinDiff);
            this.groupBoxDiff.Location = new System.Drawing.Point(650, 540);
            this.groupBoxDiff.Size = new System.Drawing.Size(300, 100);
            this.groupBoxDiff.Text = "Перепад температур";
            this.labelMaxDiff.Text = "Макс. перепад: день ?";
            this.labelMinDiff.Text = "Мин. перепад: день ?";

            // Form1
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 662);
            this.Controls.Add(this.graphPanel);
            this.Controls.Add(this.groupBoxDiff);
            this.Controls.Add(this.groupBoxRange);
            this.Controls.Add(this.groupBoxForecast);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Text = "Анализ температур";

            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.groupBoxForecast.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numericForecastDays)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericWindow)).EndInit();
            this.groupBoxRange.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackStart)).EndInit();
            this.groupBoxDiff.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // Объявления контролов
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem экспортToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBoxForecast;
        private System.Windows.Forms.Button btnForecast;
        private System.Windows.Forms.Label labelDays;
        private System.Windows.Forms.Label labelWindow;
        private System.Windows.Forms.NumericUpDown numericForecastDays;
        private System.Windows.Forms.NumericUpDown numericWindow;
        private System.Windows.Forms.GroupBox groupBoxRange;
        private System.Windows.Forms.Button btnApplyRange;
        private System.Windows.Forms.Label labelRange;
        private System.Windows.Forms.TrackBar trackEnd;
        private System.Windows.Forms.TrackBar trackStart;
        private System.Windows.Forms.GroupBox groupBoxDiff;
        private System.Windows.Forms.Label labelMaxDiff;
        private System.Windows.Forms.Label labelMinDiff;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Panel graphPanel; // вместо Chart
    }
}