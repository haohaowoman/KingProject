namespace mcLogic.Execute.Watcher
{
    partial class ExecuteWatcherDlg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            this.label1 = new System.Windows.Forms.Label();
            this.ETypeTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ENameTextBox = new System.Windows.Forms.TextBox();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.PeriodTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.PIDKpTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.PIDKiTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.PIDTdTextBox = new System.Windows.Forms.TextBox();
            this.ResetBtn = new System.Windows.Forms.Button();
            this.EOverBtn = new System.Windows.Forms.Button();
            this.EBegainBtn = new System.Windows.Forms.Button();
            this.EChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.ResetButton = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.TargetValueTextBox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.TsTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.EChart)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "ExecuterType";
            // 
            // ETypeTextBox
            // 
            this.ETypeTextBox.Location = new System.Drawing.Point(95, 20);
            this.ETypeTextBox.Name = "ETypeTextBox";
            this.ETypeTextBox.Size = new System.Drawing.Size(100, 21);
            this.ETypeTextBox.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "ExecuterName";
            // 
            // ENameTextBox
            // 
            this.ENameTextBox.Location = new System.Drawing.Point(95, 47);
            this.ENameTextBox.Name = "ENameTextBox";
            this.ENameTextBox.Size = new System.Drawing.Size(100, 21);
            this.ENameTextBox.TabIndex = 2;
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(1061, 47);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(111, 23);
            this.RefreshBtn.TabIndex = 3;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "Period";
            // 
            // PeriodTextBox
            // 
            this.PeriodTextBox.Location = new System.Drawing.Point(325, 20);
            this.PeriodTextBox.Name = "PeriodTextBox";
            this.PeriodTextBox.Size = new System.Drawing.Size(100, 21);
            this.PeriodTextBox.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(231, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "Kp";
            // 
            // PIDKpTextBox
            // 
            this.PIDKpTextBox.Location = new System.Drawing.Point(254, 47);
            this.PIDKpTextBox.Name = "PIDKpTextBox";
            this.PIDKpTextBox.Size = new System.Drawing.Size(66, 21);
            this.PIDKpTextBox.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(327, 50);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "Ti";
            // 
            // PIDKiTextBox
            // 
            this.PIDKiTextBox.Location = new System.Drawing.Point(350, 47);
            this.PIDKiTextBox.Name = "PIDKiTextBox";
            this.PIDKiTextBox.Size = new System.Drawing.Size(66, 21);
            this.PIDKiTextBox.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(424, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 12);
            this.label6.TabIndex = 1;
            this.label6.Text = "Td";
            // 
            // PIDTdTextBox
            // 
            this.PIDTdTextBox.Location = new System.Drawing.Point(447, 47);
            this.PIDTdTextBox.Name = "PIDTdTextBox";
            this.PIDTdTextBox.Size = new System.Drawing.Size(66, 21);
            this.PIDTdTextBox.TabIndex = 2;
            // 
            // ResetBtn
            // 
            this.ResetBtn.Location = new System.Drawing.Point(729, 18);
            this.ResetBtn.Name = "ResetBtn";
            this.ResetBtn.Size = new System.Drawing.Size(75, 23);
            this.ResetBtn.TabIndex = 4;
            this.ResetBtn.Text = "ResetParam";
            this.ResetBtn.UseVisualStyleBackColor = true;
            this.ResetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // EOverBtn
            // 
            this.EOverBtn.Location = new System.Drawing.Point(859, 47);
            this.EOverBtn.Name = "EOverBtn";
            this.EOverBtn.Size = new System.Drawing.Size(92, 23);
            this.EOverBtn.TabIndex = 4;
            this.EOverBtn.Text = "ExecuteOver";
            this.EOverBtn.UseVisualStyleBackColor = true;
            this.EOverBtn.Click += new System.EventHandler(this.EOverBtn_Click);
            // 
            // EBegainBtn
            // 
            this.EBegainBtn.Location = new System.Drawing.Point(729, 47);
            this.EBegainBtn.Name = "EBegainBtn";
            this.EBegainBtn.Size = new System.Drawing.Size(111, 23);
            this.EBegainBtn.TabIndex = 4;
            this.EBegainBtn.Text = "ExecuteBegain";
            this.EBegainBtn.UseVisualStyleBackColor = true;
            this.EBegainBtn.Click += new System.EventHandler(this.EBegainBtn_Click);
            // 
            // EChart
            // 
            this.EChart.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EChart.BackColor = System.Drawing.Color.Gainsboro;
            chartArea1.AxisX.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea1.AxisX.LabelStyle.Format = "{0:HH:mm:ss}";
            chartArea1.AxisX.Title = "Time";
            chartArea1.AxisY.ArrowStyle = System.Windows.Forms.DataVisualization.Charting.AxisArrowStyle.Triangle;
            chartArea1.AxisY.LabelStyle.Format = "{0:F1}";
            chartArea1.AxisY.Title = "Value";
            chartArea1.BackColor = System.Drawing.Color.LightGray;
            chartArea1.CursorX.IsUserEnabled = true;
            chartArea1.CursorX.IsUserSelectionEnabled = true;
            chartArea1.CursorX.LineWidth = 2;
            chartArea1.CursorY.IsUserEnabled = true;
            chartArea1.CursorY.IsUserSelectionEnabled = true;
            chartArea1.CursorY.LineWidth = 2;
            chartArea1.CursorY.Position = 10D;
            chartArea1.Name = "ChartArea1";
            this.EChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            legend1.Title = "Executer";
            this.EChart.Legends.Add(legend1);
            this.EChart.Location = new System.Drawing.Point(14, 74);
            this.EChart.Name = "EChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.IsValueShownAsLabel = true;
            series1.LabelFormat = "{0:F1}";
            series1.Legend = "Legend1";
            series1.LegendText = "ExeValue";
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "ExeValue";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Red;
            series2.IsValueShownAsLabel = true;
            series2.LabelFormat = "{0:f1}";
            series2.Legend = "Legend1";
            series2.LegendText = "FedbackValue";
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series2.Name = "FedbackValue";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.EChart.Series.Add(series1);
            this.EChart.Series.Add(series2);
            this.EChart.Size = new System.Drawing.Size(1160, 661);
            this.EChart.TabIndex = 5;
            this.EChart.Text = "Watcher Data Chart";
            title1.Name = "Title1";
            title1.Text = "PID调节监控器";
            this.EChart.Titles.Add(title1);
            // 
            // ResetButton
            // 
            this.ResetButton.Location = new System.Drawing.Point(967, 47);
            this.ResetButton.Name = "ResetButton";
            this.ResetButton.Size = new System.Drawing.Size(75, 23);
            this.ResetButton.TabIndex = 6;
            this.ResetButton.Text = "Reset";
            this.ResetButton.UseVisualStyleBackColor = true;
            this.ResetButton.Click += new System.EventHandler(this.ResetButton_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(525, 23);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "TargetValue";
            // 
            // TargetValueTextBox
            // 
            this.TargetValueTextBox.Location = new System.Drawing.Point(602, 20);
            this.TargetValueTextBox.Name = "TargetValueTextBox";
            this.TargetValueTextBox.Size = new System.Drawing.Size(100, 21);
            this.TargetValueTextBox.TabIndex = 2;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(525, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 12);
            this.label8.TabIndex = 1;
            this.label8.Text = "Ts";
            // 
            // TsTextBox
            // 
            this.TsTextBox.Location = new System.Drawing.Point(548, 47);
            this.TsTextBox.Name = "TsTextBox";
            this.TsTextBox.Size = new System.Drawing.Size(62, 21);
            this.TsTextBox.TabIndex = 2;
            // 
            // ExecuteWatcherDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 747);
            this.Controls.Add(this.ResetButton);
            this.Controls.Add(this.EChart);
            this.Controls.Add(this.EBegainBtn);
            this.Controls.Add(this.EOverBtn);
            this.Controls.Add(this.ResetBtn);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.PIDTdTextBox);
            this.Controls.Add(this.PIDKiTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PIDKpTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.TsTextBox);
            this.Controls.Add(this.TargetValueTextBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.PeriodTextBox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ENameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ETypeTextBox);
            this.Controls.Add(this.label1);
            this.Name = "ExecuteWatcherDlg";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ExecuteWatcherDlg";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ExecuteWatcherDlg_FormClosed);
            this.Load += new System.EventHandler(this.ExecuteWatcherDlg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ETypeTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox ENameTextBox;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PeriodTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox PIDKpTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox PIDKiTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PIDTdTextBox;
        private System.Windows.Forms.Button ResetBtn;
        private System.Windows.Forms.Button EOverBtn;
        private System.Windows.Forms.Button EBegainBtn;
        private System.Windows.Forms.DataVisualization.Charting.Chart EChart;
        private System.Windows.Forms.Button ResetButton;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TargetValueTextBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TsTextBox;
    }
}