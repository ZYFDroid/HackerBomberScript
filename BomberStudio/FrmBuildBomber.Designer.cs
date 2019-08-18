namespace BomberStudio
{
    partial class FrmBuildBomber
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
            this.label2 = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblComment = new System.Windows.Forms.Label();
            this.chkUseRGB = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.numThreadCount = new System.Windows.Forms.NumericUpDown();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.numThreadCount)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "线程数：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "生成类型";
            // 
            // cmbType
            // 
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Items.AddRange(new object[] {
            "Windows程序",
            "跨平台程序",
            "Android程序"});
            this.cmbType.Location = new System.Drawing.Point(84, 19);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(207, 20);
            this.cmbType.TabIndex = 3;
            // 
            // lblComment
            // 
            this.lblComment.Location = new System.Drawing.Point(84, 82);
            this.lblComment.Name = "lblComment";
            this.lblComment.Size = new System.Drawing.Size(207, 39);
            this.lblComment.TabIndex = 4;
            this.lblComment.Text = "32线程，非常适合轰炸。";
            // 
            // chkUseRGB
            // 
            this.chkUseRGB.AutoSize = true;
            this.chkUseRGB.Location = new System.Drawing.Point(86, 124);
            this.chkUseRGB.Name = "chkUseRGB";
            this.chkUseRGB.Size = new System.Drawing.Size(90, 16);
            this.chkUseRGB.TabIndex = 5;
            this.chkUseRGB.Text = "开启RGB特效";
            this.chkUseRGB.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 154);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(287, 38);
            this.button1.TabIndex = 6;
            this.button1.Text = "生成轰炸机";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1_Click);
            // 
            // numThreadCount
            // 
            this.numThreadCount.Location = new System.Drawing.Point(84, 50);
            this.numThreadCount.Maximum = new decimal(new int[] {
            1024,
            0,
            0,
            0});
            this.numThreadCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numThreadCount.Name = "numThreadCount";
            this.numThreadCount.Size = new System.Drawing.Size(207, 21);
            this.numThreadCount.TabIndex = 7;
            this.numThreadCount.Value = new decimal(new int[] {
            32,
            0,
            0,
            0});
            this.numThreadCount.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // FrmBuildBomber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 207);
            this.Controls.Add(this.numThreadCount);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.chkUseRGB);
            this.Controls.Add(this.lblComment);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmBuildBomber";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "生成轰炸机";
            this.Load += new System.EventHandler(this.FrmBuildBomber_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numThreadCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblComment;
        private System.Windows.Forms.CheckBox chkUseRGB;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown numThreadCount;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}