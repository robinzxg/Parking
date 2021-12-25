namespace ParkingDemo
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ethConfiggroupBox = new System.Windows.Forms.GroupBox();
            this.listenstatelabel = new System.Windows.Forms.Label();
            this.deviceIPstatelabel = new System.Windows.Forms.Label();
            this.startListenbutton = new System.Windows.Forms.Button();
            this.deviceIPlabel = new System.Windows.Forms.Label();
            this.connectstatelabel = new System.Windows.Forms.Label();
            this.label87 = new System.Windows.Forms.Label();
            this.severPorttextBox = new System.Windows.Forms.TextBox();
            this.severPortlabel = new System.Windows.Forms.Label();
            this.severIPcomboBox = new System.Windows.Forms.ComboBox();
            this.severIPlabel = new System.Windows.Forms.Label();
            this.CommSet_groupBox = new System.Windows.Forms.GroupBox();
            this.CommBaud_comboBox = new System.Windows.Forms.ComboBox();
            this.CommPort_comboBox = new System.Windows.Forms.ComboBox();
            this.OpenClosePort_Button = new System.Windows.Forms.Button();
            this.CommStatus_label = new System.Windows.Forms.Label();
            this.CommBaud_label = new System.Windows.Forms.Label();
            this.CommPort_label = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ethConfiggroupBox.SuspendLayout();
            this.CommSet_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // ethConfiggroupBox
            // 
            this.ethConfiggroupBox.Controls.Add(this.listenstatelabel);
            this.ethConfiggroupBox.Controls.Add(this.deviceIPstatelabel);
            this.ethConfiggroupBox.Controls.Add(this.startListenbutton);
            this.ethConfiggroupBox.Controls.Add(this.deviceIPlabel);
            this.ethConfiggroupBox.Controls.Add(this.connectstatelabel);
            this.ethConfiggroupBox.Controls.Add(this.label87);
            this.ethConfiggroupBox.Controls.Add(this.severPorttextBox);
            this.ethConfiggroupBox.Controls.Add(this.severPortlabel);
            this.ethConfiggroupBox.Controls.Add(this.severIPcomboBox);
            this.ethConfiggroupBox.Controls.Add(this.severIPlabel);
            this.ethConfiggroupBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ethConfiggroupBox.ForeColor = System.Drawing.Color.Black;
            this.ethConfiggroupBox.Location = new System.Drawing.Point(7, 86);
            this.ethConfiggroupBox.Name = "ethConfiggroupBox";
            this.ethConfiggroupBox.Size = new System.Drawing.Size(649, 93);
            this.ethConfiggroupBox.TabIndex = 8;
            this.ethConfiggroupBox.TabStop = false;
            this.ethConfiggroupBox.Text = "Ethernet  Port Set";
            // 
            // listenstatelabel
            // 
            this.listenstatelabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.listenstatelabel.AutoSize = true;
            this.listenstatelabel.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.listenstatelabel.ForeColor = System.Drawing.Color.DarkGray;
            this.listenstatelabel.Location = new System.Drawing.Point(484, 28);
            this.listenstatelabel.Name = "listenstatelabel";
            this.listenstatelabel.Size = new System.Drawing.Size(34, 24);
            this.listenstatelabel.TabIndex = 6;
            this.listenstatelabel.Text = "●";
            // 
            // deviceIPstatelabel
            // 
            this.deviceIPstatelabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.deviceIPstatelabel.AutoSize = true;
            this.deviceIPstatelabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceIPstatelabel.Location = new System.Drawing.Point(103, 70);
            this.deviceIPstatelabel.Name = "deviceIPstatelabel";
            this.deviceIPstatelabel.Size = new System.Drawing.Size(48, 17);
            this.deviceIPstatelabel.TabIndex = 14;
            this.deviceIPstatelabel.Text = "0.0.0.0";
            // 
            // startListenbutton
            // 
            this.startListenbutton.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startListenbutton.Location = new System.Drawing.Point(524, 27);
            this.startListenbutton.Name = "startListenbutton";
            this.startListenbutton.Size = new System.Drawing.Size(93, 27);
            this.startListenbutton.TabIndex = 10;
            this.startListenbutton.Text = "Start  Listen";
            this.startListenbutton.UseVisualStyleBackColor = true;
            this.startListenbutton.Click += new System.EventHandler(this.startListenbutton_Click);
            // 
            // deviceIPlabel
            // 
            this.deviceIPlabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.deviceIPlabel.AutoSize = true;
            this.deviceIPlabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deviceIPlabel.Location = new System.Drawing.Point(43, 70);
            this.deviceIPlabel.Name = "deviceIPlabel";
            this.deviceIPlabel.Size = new System.Drawing.Size(57, 17);
            this.deviceIPlabel.TabIndex = 13;
            this.deviceIPlabel.Text = "WDC  IP:";
            // 
            // connectstatelabel
            // 
            this.connectstatelabel.AutoSize = true;
            this.connectstatelabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.connectstatelabel.ForeColor = System.Drawing.Color.Black;
            this.connectstatelabel.Location = new System.Drawing.Point(335, 70);
            this.connectstatelabel.Name = "connectstatelabel";
            this.connectstatelabel.Size = new System.Drawing.Size(116, 17);
            this.connectstatelabel.TabIndex = 12;
            this.connectstatelabel.Text = "Waiting  Connect...";
            // 
            // label87
            // 
            this.label87.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label87.AutoSize = true;
            this.label87.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label87.Location = new System.Drawing.Point(238, 70);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(94, 17);
            this.label87.TabIndex = 11;
            this.label87.Text = "Connect  State:";
            // 
            // severPorttextBox
            // 
            this.severPorttextBox.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.severPorttextBox.Location = new System.Drawing.Point(338, 28);
            this.severPorttextBox.Name = "severPorttextBox";
            this.severPorttextBox.Size = new System.Drawing.Size(129, 25);
            this.severPorttextBox.TabIndex = 9;
            this.severPorttextBox.Text = "6000";
            // 
            // severPortlabel
            // 
            this.severPortlabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.severPortlabel.AutoSize = true;
            this.severPortlabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.severPortlabel.Location = new System.Drawing.Point(254, 32);
            this.severPortlabel.Name = "severPortlabel";
            this.severPortlabel.Size = new System.Drawing.Size(78, 17);
            this.severPortlabel.TabIndex = 8;
            this.severPortlabel.Text = "Server  Port:";
            // 
            // severIPcomboBox
            // 
            this.severIPcomboBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.severIPcomboBox.Font = new System.Drawing.Font("Calibri", 10.5F);
            this.severIPcomboBox.FormattingEnabled = true;
            this.severIPcomboBox.Items.AddRange(new object[] {
            "192.168.1.2",
            "119.78.248.11",
            "192.168.1.103",
            "192.168.100.106"});
            this.severIPcomboBox.Location = new System.Drawing.Point(106, 28);
            this.severIPcomboBox.Name = "severIPcomboBox";
            this.severIPcomboBox.Size = new System.Drawing.Size(129, 25);
            this.severIPcomboBox.TabIndex = 7;
            // 
            // severIPlabel
            // 
            this.severIPlabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.severIPlabel.AutoSize = true;
            this.severIPlabel.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.severIPlabel.Location = new System.Drawing.Point(35, 32);
            this.severIPlabel.Name = "severIPlabel";
            this.severIPlabel.Size = new System.Drawing.Size(65, 17);
            this.severIPlabel.TabIndex = 6;
            this.severIPlabel.Text = "Server  IP:";
            // 
            // CommSet_groupBox
            // 
            this.CommSet_groupBox.AutoSize = true;
            this.CommSet_groupBox.Controls.Add(this.CommBaud_comboBox);
            this.CommSet_groupBox.Controls.Add(this.CommPort_comboBox);
            this.CommSet_groupBox.Controls.Add(this.OpenClosePort_Button);
            this.CommSet_groupBox.Controls.Add(this.CommStatus_label);
            this.CommSet_groupBox.Controls.Add(this.CommBaud_label);
            this.CommSet_groupBox.Controls.Add(this.CommPort_label);
            this.CommSet_groupBox.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommSet_groupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CommSet_groupBox.Location = new System.Drawing.Point(7, 7);
            this.CommSet_groupBox.Name = "CommSet_groupBox";
            this.CommSet_groupBox.Size = new System.Drawing.Size(649, 73);
            this.CommSet_groupBox.TabIndex = 7;
            this.CommSet_groupBox.TabStop = false;
            this.CommSet_groupBox.Text = "Comm  Port  Set";
            // 
            // CommBaud_comboBox
            // 
            this.CommBaud_comboBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CommBaud_comboBox.AutoCompleteCustomSource.AddRange(new string[] {
            "110",
            "300",
            "600",
            "1200",
            "2400",
            "4800",
            "9600",
            "14400",
            "19200",
            "38400",
            "56000",
            "57600",
            "115200",
            "128000",
            "230400",
            "256000"});
            this.CommBaud_comboBox.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommBaud_comboBox.FormattingEnabled = true;
            this.CommBaud_comboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CommBaud_comboBox.Items.AddRange(new object[] {
            "2400",
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200"});
            this.CommBaud_comboBox.Location = new System.Drawing.Point(338, 26);
            this.CommBaud_comboBox.Name = "CommBaud_comboBox";
            this.CommBaud_comboBox.Size = new System.Drawing.Size(129, 25);
            this.CommBaud_comboBox.TabIndex = 5;
            // 
            // CommPort_comboBox
            // 
            this.CommPort_comboBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CommPort_comboBox.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommPort_comboBox.FormattingEnabled = true;
            this.CommPort_comboBox.Location = new System.Drawing.Point(109, 26);
            this.CommPort_comboBox.Name = "CommPort_comboBox";
            this.CommPort_comboBox.Size = new System.Drawing.Size(129, 25);
            this.CommPort_comboBox.TabIndex = 4;
            // 
            // OpenClosePort_Button
            // 
            this.OpenClosePort_Button.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.OpenClosePort_Button.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenClosePort_Button.Location = new System.Drawing.Point(524, 25);
            this.OpenClosePort_Button.Name = "OpenClosePort_Button";
            this.OpenClosePort_Button.Size = new System.Drawing.Size(93, 27);
            this.OpenClosePort_Button.TabIndex = 3;
            this.OpenClosePort_Button.Text = "Open  Comm";
            this.OpenClosePort_Button.UseVisualStyleBackColor = true;
            this.OpenClosePort_Button.Click += new System.EventHandler(this.OpenClosePort_Button_Click);
            // 
            // CommStatus_label
            // 
            this.CommStatus_label.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CommStatus_label.AutoSize = true;
            this.CommStatus_label.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CommStatus_label.ForeColor = System.Drawing.Color.DarkGray;
            this.CommStatus_label.Location = new System.Drawing.Point(484, 26);
            this.CommStatus_label.Name = "CommStatus_label";
            this.CommStatus_label.Size = new System.Drawing.Size(34, 24);
            this.CommStatus_label.TabIndex = 2;
            this.CommStatus_label.Text = "●";
            // 
            // CommBaud_label
            // 
            this.CommBaud_label.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CommBaud_label.AutoSize = true;
            this.CommBaud_label.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommBaud_label.Location = new System.Drawing.Point(258, 30);
            this.CommBaud_label.Name = "CommBaud_label";
            this.CommBaud_label.Size = new System.Drawing.Size(74, 17);
            this.CommBaud_label.TabIndex = 1;
            this.CommBaud_label.Text = "Baud  Rate:";
            // 
            // CommPort_label
            // 
            this.CommPort_label.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CommPort_label.AutoSize = true;
            this.CommPort_label.Font = new System.Drawing.Font("Calibri", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CommPort_label.Location = new System.Drawing.Point(27, 30);
            this.CommPort_label.Name = "CommPort_label";
            this.CommPort_label.Size = new System.Drawing.Size(76, 17);
            this.CommPort_label.TabIndex = 0;
            this.CommPort_label.Text = "Port  Name:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 206);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(640, 253);
            this.richTextBox1.TabIndex = 9;
            this.richTextBox1.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(9, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(649, 280);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Raw Data";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(662, 467);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ethConfiggroupBox);
            this.Controls.Add(this.CommSet_groupBox);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "ParkDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ethConfiggroupBox.ResumeLayout(false);
            this.ethConfiggroupBox.PerformLayout();
            this.CommSet_groupBox.ResumeLayout(false);
            this.CommSet_groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox ethConfiggroupBox;
        private System.Windows.Forms.Label listenstatelabel;
        private System.Windows.Forms.Label deviceIPstatelabel;
        private System.Windows.Forms.Button startListenbutton;
        private System.Windows.Forms.Label deviceIPlabel;
        private System.Windows.Forms.Label connectstatelabel;
        private System.Windows.Forms.Label label87;
        private System.Windows.Forms.TextBox severPorttextBox;
        private System.Windows.Forms.Label severPortlabel;
        private System.Windows.Forms.ComboBox severIPcomboBox;
        private System.Windows.Forms.Label severIPlabel;
        private System.Windows.Forms.GroupBox CommSet_groupBox;
        private System.Windows.Forms.ComboBox CommBaud_comboBox;
        private System.Windows.Forms.ComboBox CommPort_comboBox;
        private System.Windows.Forms.Button OpenClosePort_Button;
        private System.Windows.Forms.Label CommStatus_label;
        private System.Windows.Forms.Label CommBaud_label;
        private System.Windows.Forms.Label CommPort_label;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

