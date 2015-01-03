namespace Tesztek
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.close = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.statuslabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.minutes = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.time = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(110, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(1)))), ((int)(((byte)(81)))), ((int)(((byte)(111)))));
            this.panel1.Controls.Add(this.close);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(300, 30);
            this.panel1.TabIndex = 1;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // close
            // 
            this.close.BackgroundImage = global::Tesztek.Properties.Resources.Untitled_2;
            this.close.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.close.Image = global::Tesztek.Properties.Resources.Untitled_2;
            this.close.Location = new System.Drawing.Point(272, 3);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(25, 23);
            this.close.TabIndex = 2;
            this.close.UseVisualStyleBackColor = true;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.label1.Font = new System.Drawing.Font("Arial", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(13, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "L# Update Checker";
            // 
            // statuslabel
            // 
            this.statuslabel.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.statuslabel.Location = new System.Drawing.Point(50, 50);
            this.statuslabel.Name = "statuslabel";
            this.statuslabel.Size = new System.Drawing.Size(200, 30);
            this.statuslabel.TabIndex = 2;
            this.statuslabel.Text = "OUTDATED";
            this.statuslabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 125);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Check every";
            // 
            // minutes
            // 
            this.minutes.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.minutes.Location = new System.Drawing.Point(79, 122);
            this.minutes.MaxLength = 3;
            this.minutes.Name = "minutes";
            this.minutes.Size = new System.Drawing.Size(33, 20);
            this.minutes.TabIndex = 4;
            this.minutes.Text = "2";
            this.minutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "min";
            // 
            // time
            // 
            this.time.AutoSize = true;
            this.time.Location = new System.Drawing.Point(232, 126);
            this.time.Name = "time";
            this.time.Size = new System.Drawing.Size(49, 13);
            this.time.TabIndex = 6;
            this.time.Text = "16:24:24";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(203, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Last:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 150);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.time);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.minutes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statuslabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "L# Checker";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Label statuslabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox minutes;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label time;
        private System.Windows.Forms.Label label4;
    }
}

