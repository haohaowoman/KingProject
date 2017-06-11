namespace LabMCESystem.Servers.HS.Calibration
{
    partial class Calibration
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
            this.label1 = new System.Windows.Forms.Label();
            this.ChannelsCombox = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.MemBtn2 = new System.Windows.Forms.Button();
            this.MemBtn1 = new System.Windows.Forms.Button();
            this.X1TextBox = new System.Windows.Forms.TextBox();
            this.X0TextBox = new System.Windows.Forms.TextBox();
            this.Y1TextBox = new System.Windows.Forms.TextBox();
            this.Y0TextBox = new System.Windows.Forms.TextBox();
            this.CalTextBox = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CSrcTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SaveBtn = new System.Windows.Forms.Button();
            this.CancleBtn = new System.Windows.Forms.Button();
            this.SampleBtn1 = new System.Windows.Forms.Button();
            this.SampleBtn2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "选择需要校准的通道:";
            // 
            // ChannelsCombox
            // 
            this.ChannelsCombox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ChannelsCombox.FormattingEnabled = true;
            this.ChannelsCombox.Location = new System.Drawing.Point(138, 10);
            this.ChannelsCombox.MaxDropDownItems = 20;
            this.ChannelsCombox.Name = "ChannelsCombox";
            this.ChannelsCombox.Size = new System.Drawing.Size(121, 20);
            this.ChannelsCombox.TabIndex = 1;
            this.ChannelsCombox.SelectedIndexChanged += new System.EventHandler(this.ChannelsCombox_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.RefreshBtn);
            this.groupBox1.Controls.Add(this.MemBtn2);
            this.groupBox1.Controls.Add(this.SampleBtn2);
            this.groupBox1.Controls.Add(this.SampleBtn1);
            this.groupBox1.Controls.Add(this.MemBtn1);
            this.groupBox1.Controls.Add(this.X1TextBox);
            this.groupBox1.Controls.Add(this.X0TextBox);
            this.groupBox1.Controls.Add(this.Y1TextBox);
            this.groupBox1.Controls.Add(this.Y0TextBox);
            this.groupBox1.Controls.Add(this.CalTextBox);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.CSrcTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(515, 262);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "通道数据";
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(425, 223);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(75, 23);
            this.RefreshBtn.TabIndex = 3;
            this.RefreshBtn.Text = "更新";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            this.RefreshBtn.Click += new System.EventHandler(this.RefreshBtn_Click);
            // 
            // MemBtn2
            // 
            this.MemBtn2.Location = new System.Drawing.Point(425, 179);
            this.MemBtn2.Name = "MemBtn2";
            this.MemBtn2.Size = new System.Drawing.Size(75, 23);
            this.MemBtn2.TabIndex = 2;
            this.MemBtn2.Text = "记录2";
            this.MemBtn2.UseVisualStyleBackColor = true;
            this.MemBtn2.Click += new System.EventHandler(this.MemBtn2_Click);
            // 
            // MemBtn1
            // 
            this.MemBtn1.Location = new System.Drawing.Point(425, 123);
            this.MemBtn1.Name = "MemBtn1";
            this.MemBtn1.Size = new System.Drawing.Size(75, 23);
            this.MemBtn1.TabIndex = 2;
            this.MemBtn1.Text = "记录1";
            this.MemBtn1.UseVisualStyleBackColor = true;
            this.MemBtn1.Click += new System.EventHandler(this.MemBtn1_Click);
            // 
            // X1TextBox
            // 
            this.X1TextBox.Location = new System.Drawing.Point(309, 181);
            this.X1TextBox.Name = "X1TextBox";
            this.X1TextBox.Size = new System.Drawing.Size(100, 21);
            this.X1TextBox.TabIndex = 1;
            // 
            // X0TextBox
            // 
            this.X0TextBox.Location = new System.Drawing.Point(309, 125);
            this.X0TextBox.Name = "X0TextBox";
            this.X0TextBox.Size = new System.Drawing.Size(100, 21);
            this.X0TextBox.TabIndex = 1;
            // 
            // Y1TextBox
            // 
            this.Y1TextBox.Location = new System.Drawing.Point(101, 181);
            this.Y1TextBox.Name = "Y1TextBox";
            this.Y1TextBox.Size = new System.Drawing.Size(100, 21);
            this.Y1TextBox.TabIndex = 1;
            // 
            // Y0TextBox
            // 
            this.Y0TextBox.Location = new System.Drawing.Point(101, 125);
            this.Y0TextBox.Name = "Y0TextBox";
            this.Y0TextBox.Size = new System.Drawing.Size(100, 21);
            this.Y0TextBox.TabIndex = 1;
            // 
            // CalTextBox
            // 
            this.CalTextBox.Location = new System.Drawing.Point(137, 53);
            this.CalTextBox.Name = "CalTextBox";
            this.CalTextBox.Size = new System.Drawing.Size(100, 21);
            this.CalTextBox.TabIndex = 1;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(214, 184);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(89, 12);
            this.label9.TabIndex = 0;
            this.label9.Text = "采样值（mA）：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(214, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 0;
            this.label6.Text = "采样值（mA）：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 157);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 0;
            this.label8.Text = "样点2：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 101);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "样点1：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 184);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 12);
            this.label7.TabIndex = 0;
            this.label7.Text = "标准源（mA）：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 128);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 0;
            this.label5.Text = "标准源（mA）：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 0;
            this.label3.Text = "通道校准值（mA）：";
            // 
            // CSrcTextBox
            // 
            this.CSrcTextBox.Location = new System.Drawing.Point(137, 26);
            this.CSrcTextBox.Name = "CSrcTextBox";
            this.CSrcTextBox.Size = new System.Drawing.Size(100, 21);
            this.CSrcTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 0;
            this.label2.Text = "通道采集源值（mA）：";
            // 
            // SaveBtn
            // 
            this.SaveBtn.Location = new System.Drawing.Point(348, 313);
            this.SaveBtn.Name = "SaveBtn";
            this.SaveBtn.Size = new System.Drawing.Size(75, 23);
            this.SaveBtn.TabIndex = 3;
            this.SaveBtn.Text = "保存";
            this.SaveBtn.UseVisualStyleBackColor = true;
            this.SaveBtn.Click += new System.EventHandler(this.SaveBtn_Click);
            // 
            // CancleBtn
            // 
            this.CancleBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancleBtn.Location = new System.Drawing.Point(439, 313);
            this.CancleBtn.Name = "CancleBtn";
            this.CancleBtn.Size = new System.Drawing.Size(75, 23);
            this.CancleBtn.TabIndex = 3;
            this.CancleBtn.Text = "退出";
            this.CancleBtn.UseVisualStyleBackColor = true;
            this.CancleBtn.Click += new System.EventHandler(this.CancleBtn_Click);
            // 
            // SampleBtn1
            // 
            this.SampleBtn1.Location = new System.Drawing.Point(322, 101);
            this.SampleBtn1.Name = "SampleBtn1";
            this.SampleBtn1.Size = new System.Drawing.Size(75, 23);
            this.SampleBtn1.TabIndex = 2;
            this.SampleBtn1.Text = "取样";
            this.SampleBtn1.UseVisualStyleBackColor = true;
            this.SampleBtn1.Click += new System.EventHandler(this.SampleBtn1_Click);
            // 
            // SampleBtn2
            // 
            this.SampleBtn2.Location = new System.Drawing.Point(322, 157);
            this.SampleBtn2.Name = "SampleBtn2";
            this.SampleBtn2.Size = new System.Drawing.Size(75, 23);
            this.SampleBtn2.TabIndex = 2;
            this.SampleBtn2.Text = "取样";
            this.SampleBtn2.UseVisualStyleBackColor = true;
            this.SampleBtn2.Click += new System.EventHandler(this.SampleBtn2_Click);
            // 
            // Calibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancleBtn;
            this.ClientSize = new System.Drawing.Size(541, 348);
            this.Controls.Add(this.CancleBtn);
            this.Controls.Add(this.SaveBtn);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ChannelsCombox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Calibration";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数采箱通道校准";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox ChannelsCombox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button MemBtn2;
        private System.Windows.Forms.Button MemBtn1;
        private System.Windows.Forms.TextBox X1TextBox;
        private System.Windows.Forms.TextBox X0TextBox;
        private System.Windows.Forms.TextBox Y1TextBox;
        private System.Windows.Forms.TextBox Y0TextBox;
        private System.Windows.Forms.TextBox CalTextBox;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox CSrcTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.Button SaveBtn;
        private System.Windows.Forms.Button CancleBtn;
        private System.Windows.Forms.Button SampleBtn2;
        private System.Windows.Forms.Button SampleBtn1;
    }
}