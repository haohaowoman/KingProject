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
            System.Windows.Forms.DataVisualization.Charting.CustomLabel customLabel1 = new System.Windows.Forms.DataVisualization.Charting.CustomLabel();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.EChart = new System.Windows.Forms.DataVisualization.Charting.Chart();
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
            ((System.ComponentModel.ISupportInitialize)(this.EChart)).BeginInit();
            this.SuspendLayout();
            // 
            // EChart
            // 
            this.EChart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            customLabel1.Text = "DateTime";
            chartArea1.AxisX.CustomLabels.Add(customLabel1);
            chartArea1.AxisX.Title = "DateTime";
            chartArea1.Name = "ChartArea1";
            this.EChart.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            legend1.Title = "Executer";
            this.EChart.Legends.Add(legend1);
            this.EChart.Location = new System.Drawing.Point(14, 95);
            this.EChart.Name = "EChart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.IsValueShownAsLabel = true;
            series1.Legend = "Legend1";
            series1.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
            series1.Name = "ExeValue";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Color = System.Drawing.Color.Red;
            series2.Legend = "Legend1";
            series2.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Diamond;
            series2.Name = "FedbackValue";
            this.EChart.Series.Add(series1);
            this.EChart.Series.Add(series2);
            this.EChart.Size = new System.Drawing.Size(566, 304);
            this.EChart.TabIndex = 0;
            this.EChart.Text = "Watcher Data Chart";
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
            this.RefreshBtn.Location = new System.Drawing.Point(521, 66);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(59, 23);
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
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "ExecuterPeriod";
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
            // ExecuteWatcherDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(590, 411);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.PIDTdTextBox);
            this.Controls.Add(this.PIDKiTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.PIDKpTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.PeriodTextBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ENameTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ETypeTextBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EChart);
            this.MaximizeBox = false;
            this.Name = "ExecuteWatcherDlg";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ExecuteWatcherDlg";
            this.Load += new System.EventHandler(this.ExecuteWatcherDlg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.EChart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart EChart;
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
    }
}