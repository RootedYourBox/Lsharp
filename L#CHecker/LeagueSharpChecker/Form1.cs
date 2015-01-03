using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using cb = System.Windows.Forms.Clipboard;
using System.Timers;
using System.Threading;
using System.Net;
using System.Collections.Specialized;

namespace Tesztek
{
    public partial class Form1 : Form
    {
        int XP;
        int yp;
        System.Drawing.Point NewPoint;
        public Form1()
        {
            InitializeComponent();
            time.Text = DateTime.Now.ToString("HH:mm:ss");
            statuslabel.Text = "OUTDATED";
            statuslabel.ForeColor = Color.Red;
            GetStatus();
        }

        public void Alert()
        {
            NotifyIcon notifyIcon1 = new NotifyIcon();
            notifyIcon1.BalloonTipText = "L# is probably updated, check it!";
            notifyIcon1.BalloonTipTitle = "L#Checker";
            notifyIcon1.Icon = new Icon("appicon.ico");
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000);

            if (statuslabel.Text == "UPDATED")
            {
                for (int i = 0; i < 5; i++)
                {
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(1000);
                    System.Media.SystemSounds.Hand.Play();
                }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            
            double min = double.Parse(minutes.Text);
            min = (int)Math.Ceiling(min);
                    var timer = new System.Threading.Timer(
                    t => GetStatus(),
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(min*60));
                    Console.WriteLine(min);
                    button1.Text = "Checking";
        }
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            XP = MousePosition.X - this.Location.X;
            yp = MousePosition.Y - this.Location.Y;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NewPoint = MousePosition;
                NewPoint.X = NewPoint.X - XP;
                NewPoint.Y = NewPoint.Y - yp;
                this.Location = NewPoint;
            }
        }
   

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private async void GetStatus()
        {
	//login the site and get the text from the shoutbox
            System.Uri uri = new System.Uri("http://www.niratisnordkyn.com/DontDeleteThis/ls.php");
            string data = await DownloadStringAsync(uri);
            Console.WriteLine(data);
            this.Invoke((MethodInvoker)delegate
                {
                    statuslabel.Text = "OUTDATED";
                    statuslabel.ForeColor = Color.Red;
                });
            if (!data.Contains("OUTDATED") && data.Length>5)
            {
                this.Invoke((MethodInvoker)delegate
                    {
                        statuslabel.Text = "UPDATED";
                        statuslabel.ForeColor = Color.ForestGreen;
                        Alert();
                    });
            }
            this.Invoke((MethodInvoker)delegate
                {
                time.Text = DateTime.Now.ToString("HH:mm:ss");
                });
            Console.WriteLine("Checked");
            
        }
        public static Task<string> DownloadStringAsync(Uri url)
        {

            var tcs = new TaskCompletionSource<string>();
            var wc = new WebClient();
            wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
            wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
            wc.Encoding = Encoding.UTF8;
            wc.DownloadStringCompleted += (s, e) =>
            {
                if (e.Error != null) tcs.TrySetException(e.Error);
                else if (e.Cancelled) tcs.TrySetCanceled();
                else tcs.TrySetResult(e.Result);
            };
            wc.DownloadStringAsync(url);
            
            return tcs.Task;
        } 

    }
}
