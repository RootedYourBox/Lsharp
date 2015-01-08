using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace GetTokenGG
{
    public partial class frmGetToken : Form
    {
        public frmGetToken()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            int processid = 0;
            Process[] processlist = Process.GetProcesses();
            foreach (Process theprocess in processlist)
            {
                if (theprocess.ProcessName == "lol")
                    //Console.WriteLine(theprocess.ProcessName + "-" + theprocess.Id);
                    processid = theprocess.Id;
            }

            string wmiQuery = string.Format("select Name, CommandLine from Win32_Process where ProcessID='{0}'", processid);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection processList = searcher.Get();
            foreach (ManagementObject process in processList)
            {
                string result = process["CommandLine"] == null ? "" : process["CommandLine"].ToString();
                //Console.WriteLine("{0} - {1}", process["Name"], process["CommandLine"]);
                string Username = cmbUsername.Text;
                string UID = cmbUID.Text;
                string Queuetype = cmbQueueType.Text;
                string account = string.Format("{0}|{1}|{2}|{3}", Username, UID, result.Trim(), Queuetype);
                StreamWriter wr = new StreamWriter(@"accounts.txt");
                wr.WriteLine(account);
                wr.Close();
                MessageBox.Show("Done!");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmGetToken_Load(object sender, EventArgs e)
        {

        }

        
        
    }
}
