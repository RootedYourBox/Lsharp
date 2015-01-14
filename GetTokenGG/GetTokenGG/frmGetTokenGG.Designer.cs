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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmGetToken));
            this.label1 = new System.Windows.Forms.Label();
            this.cmbUsername = new System.Windows.Forms.ComboBox();
            this.cmbUID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkRemember = new System.Windows.Forms.CheckBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.cmbQueueType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.timeBug = new System.Windows.Forms.Timer(this.components);
            this.lblDone = new System.Windows.Forms.Label();
            this.Clock = new System.Windows.Forms.Timer(this.components);
            this.btnGet = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // cmbUsername
            // 
            this.cmbUsername.FormattingEnabled = true;
            this.cmbUsername.Location = new System.Drawing.Point(85, 12);
            this.cmbUsername.Name = "cmbUsername";
            this.cmbUsername.Size = new System.Drawing.Size(121, 21);
            this.cmbUsername.TabIndex = 1;
            this.cmbUsername.SelectedIndexChanged += new System.EventHandler(this.cmbUsername_SelectedIndexChanged);
            this.cmbUsername.TextUpdate += new System.EventHandler(this.cmbUsername_TextUpdate);
            // 
            // cmbUID
            // 
            this.cmbUID.FormattingEnabled = true;
            this.cmbUID.Location = new System.Drawing.Point(85, 49);
            this.cmbUID.Name = "cmbUID";
            this.cmbUID.Size = new System.Drawing.Size(121, 21);
            this.cmbUID.TabIndex = 3;
            this.cmbUID.SelectedIndexChanged += new System.EventHandler(this.cmbUID_SelectedIndexChanged);
            this.cmbUID.TextUpdate += new System.EventHandler(this.cmbUID_TextUpdate);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "UID:";
            // 
            // chkRemember
            // 
            this.chkRemember.AutoSize = true;
            this.chkRemember.Checked = true;
            this.chkRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRemember.Location = new System.Drawing.Point(53, 115);
            this.chkRemember.Name = "chkRemember";
            this.chkRemember.Size = new System.Drawing.Size(77, 17);
            this.chkRemember.TabIndex = 4;
            this.chkRemember.Text = "Remember";
            this.chkRemember.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(225, 12);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(90, 35);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
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
            this.cmbQueueType.Location = new System.Drawing.Point(85, 86);
            this.cmbQueueType.Name = "cmbQueueType";
            this.cmbQueueType.Size = new System.Drawing.Size(121, 21);
            this.cmbQueueType.TabIndex = 6;
            this.cmbQueueType.Text = "MEDIUM_BOT";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "QueueType:";
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(225, 94);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(90, 35);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // timeBug
            // 
            this.timeBug.Enabled = true;
            this.timeBug.Interval = 200;
            this.timeBug.Tick += new System.EventHandler(this.timeBug_Tick);
            // 
            // lblDone
            // 
            this.lblDone.AutoSize = true;
            this.lblDone.ForeColor = System.Drawing.Color.Red;
            this.lblDone.Location = new System.Drawing.Point(170, 116);
            this.lblDone.Name = "lblDone";
            this.lblDone.Size = new System.Drawing.Size(36, 13);
            this.lblDone.TabIndex = 9;
            this.lblDone.Text = "Done!";
            this.lblDone.Visible = false;
            // 
            // Clock
            // 
            this.Clock.Interval = 2000;
            this.Clock.Tick += new System.EventHandler(this.Clock_Tick);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(225, 53);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(90, 35);
            this.btnGet.TabIndex = 10;
            this.btnGet.Text = "Get Token";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // frmGetToken
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 140);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.lblDone);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbQueueType);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.chkRemember);
            this.Controls.Add(this.cmbUID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cmbUsername);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
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
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.ComboBox cmbQueueType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer timeBug;
        private System.Windows.Forms.Label lblDone;
        private System.Windows.Forms.Timer Clock;
        private System.Windows.Forms.Button btnGet;
    }
}

