using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Data.SQLite;

namespace GetTokenGG
{
    public partial class frmGetToken : Form
    {
        #region Fields
        SQLiteConnection conn = new SQLiteConnection("Data source = account.db");
        SQLiteCommand cmd = new SQLiteCommand();

        SQLiteDataAdapter adapter = new SQLiteDataAdapter();

        DataTable dt = new DataTable();
        BindingSource bs = new BindingSource();
        int i = 0;

        #endregion

        public frmGetToken()
        {
            InitializeComponent();
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (chkRemember.Checked == true)
            {
                SaveData();
            }
            else
            {
                DelData();
            }

            GetToken();
           
            LoadUsername();
            LoadUID();
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //GarenaPlus();
            this.Close();
        }

        private void DelData()
        {
            conn.Open();
            DataRow dr = ((DataRowView)bs.Current).Row;
            string Uname = dr["Username"].ToString();
            if (cmbUsername.Text == Uname)
            {
                cmd.CommandText = "delete from account where Username = '" + Uname + "'";
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        private void SaveData()
        {
            if (CheckData(cmbUsername.Text) || CheckData(cmbUID.Text))
            {
                cmd.CommandText = "update account set Username = '" + cmbUsername.Text + "', UID = '" + cmbUID.Text + "' Where Username = '" + cmbUsername + "'";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            else
            {
                cmd.CommandText = "Insert Into account(Username,UID) Values ('" + cmbUsername.Text + "','" + cmbUID.Text + "')";
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            conn.Close();
        }

        private bool CheckData(String Name)
        {
            bool Check;
            conn.Open();
            cmd.CommandText = ("Select * From account where Username = '" + Name + "'");
            SQLiteDataReader dr = cmd.ExecuteReader();
            
            if (dr.Read())
                { Check = true; }
            else
                { Check = false; }
            
            dr.Close();
            conn.Close();
            return Check;
        }

        private void GetData(SQLiteCommand sql)
        {
            
            cmd.Connection = conn;
            adapter.SelectCommand = cmd;
            cmd.CommandText = sql.CommandText;
            adapter.SelectCommand = cmd;
            dt.Rows.Clear();
            adapter.Fill(dt);
            bs.DataSource = dt;

            
        }
        private void LoadUsername()
        {
            SQLiteCommand u_sql = new SQLiteCommand();
            u_sql.CommandText = "select * from account";
            GetData(u_sql);
            cmbUsername.DataSource = bs;
            cmbUsername.DisplayMember = "Username";
            cmbUsername.ValueMember = "ID";

        }

        private void LoadUID()
        {
            SQLiteCommand u_sql = new SQLiteCommand();
            u_sql.CommandText = "select * from account";
            GetData(u_sql);
            cmbUID.DataSource = bs;
            cmbUID.DisplayMember = "UID";
            cmbUID.ValueMember = "ID";
        }


        private void GetToken()
        {
            int processid = 0;
            try
            {
                foreach (Process proc in Process.GetProcessesByName("lol"))
                {
                    processid = proc.Id;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            string wmiQuery = string.Format("select Name, CommandLine from Win32_Process where ProcessID='{0}'", processid);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
            ManagementObjectCollection processList = searcher.Get();
            foreach (ManagementObject process in processList)
            {
                string result = process["CommandLine"] == null ? "" : process["CommandLine"].ToString();
                string Username = cmbUsername.Text;
                string UID = cmbUID.Text;
                string Queuetype = cmbQueueType.Text;
                string account = string.Format("{0}|{1}|{2}|{3}", Username, UID, result.Trim(), Queuetype);
                StreamWriter wr = new StreamWriter(@"accounts.txt");
                wr.WriteLine(account);
                wr.Close();
                MessageBox.Show("Done!", Username);
            }
        }

        private void GarenaPlus()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("GarenaMessenger"))
                {
                    //MessageBox.Show(proc.ProcessName + "-" + proc.Id);
                   
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void frmGetToken_Load(object sender, EventArgs e)
        {
            LoadUsername();
            LoadUID();
            timeBug.Enabled = true;
        }


        private void cmbUsername_TextUpdate(object sender, EventArgs e)
        {
            chkRemember.Checked = false;
        }

        private void cmbUID_TextUpdate(object sender, EventArgs e)
        {
            chkRemember.Checked = false;
        }

        private void cmbUsername_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkRemember.Checked = true;
        }

        private void cmbUID_SelectedIndexChanged(object sender, EventArgs e)
        {
            chkRemember.Checked = true;
        }


        private void timeBug_Tick(object sender, EventArgs e)
        {
            
            i++;
            if (i == 60) i = 0;
            this.Text = "Get Garena Token by Banana " + i.ToString();
            try
            {
                foreach (Process proc in Process.GetProcessesByName("BsSndRpt"))
                {
                    proc.Kill();
                    GarenaPlus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        
    }
}