namespace GetTokenGG
{
    partial class frmGetToken
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGetToken));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbUsername = new System.Windows.Forms.ComboBox();
            this.cmbUID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.btnGet = new System.Windows.Forms.Button();
            this.cmbQueueType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // cmbUsername
            // 
            this.cmbUsername.FormattingEnabled = true;
            this.cmbUsername.Items.AddRange(new object[] {
            "vnice91",
            "cappy35"});
            this.cmbUsername.Location = new System.Drawing.Point(76, 6);
            this.cmbUsername.Name = "cmbUsername";
            this.cmbUsername.Size = new System.Drawing.Size(121, 21);
            this.cmbUsername.TabIndex = 1;
            this.cmbUsername.Text = "vnice91";
            // 
            // cmbUID
            // 
            this.cmbUID.FormattingEnabled = true;
            this.cmbUID.Items.AddRange(new object[] {
            "180157458",
            "86887334"});
            this.cmbUID.Location = new System.Drawing.Point(76, 33);
            this.cmbUID.Name = "cmbUID";
            this.cmbUID.Size = new System.Drawing.Size(121, 21);
            this.cmbUID.TabIndex = 3;
            this.cmbUID.Text = "180157458";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "UID:";
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Location = new System.Drawing.Point(76, 87);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(77, 17);
            this.chkRemember.TabIndex = 4;
            this.chkRemember.Text = "Remember";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(218, 6);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(75, 43);
            this.btnGet.TabIndex = 5;
            this.btnGet.Text = "Get token";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // cmbQueueType
            // 
            this.cmbQueueType.FormattingEnabled = true;
            this.cmbQueueType.Items.AddRange(new object[] {
            "NORMAL_5x5",
            "NORMAL_3x3",
            "ARAM",
            "INTRO_BOT",
            "BEGINNER_BOT",
            "MEDIUM_BOT"});
            this.cmbQueueType.Location = new System.Drawing.Point(76, 60);
            this.cmbQueueType.Name = "cmbQueueType";
            this.cmbQueueType.Size = new System.Drawing.Size(121, 21);
            this.cmbQueueType.TabIndex = 6;
            this.cmbQueueType.Text = "MEDIUM_BOT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 63);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "QueueType:";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(218, 55);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 43);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // frmGetToken
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(309, 114);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbQueueType);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.cmbUID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbUsername);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmGetToken";
            this.Text = "Get Garena Token by Banana";
            this.Load += new System.EventHandler(this.frmGetToken_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbUsername;
        private System.Windows.Forms.ComboBox cmbUID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkRemember;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.ComboBox cmbQueueType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExit;
    }
}

