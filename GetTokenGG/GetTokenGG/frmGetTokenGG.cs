#region Using

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
using System.Security.Cryptography;

#endregion

namespace GetTokenGG
{
    public partial class frmGetToken : Form
    {
        #region Fields
        SQLiteConnection conn = new SQLiteConnection("Data source = config/account.db");
        SQLiteCommand cmd = new SQLiteCommand();
        SQLiteDataAdapter adapter = new SQLiteDataAdapter();

        DataTable dt = new DataTable();
        BindingSource bs = new BindingSource();
        Process procID;
        //int j = 0;

        #endregion

        public frmGetToken()
        {
            InitializeComponent();
        }

        #region Data
        //-----------------------------------------------------------------------------------------------------------------
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
                cmd.CommandText = "update account set Username = '" + cmbUsername.Text + "', UID = '" + cmbUID.Text + "' Where Username = '" + cmbUsername.Text + "'";
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
        //==============================================================================================================
        #endregion 

        #region Event

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (chkRemember.Checked == true)
            {
                SaveData();
            }
            else
            {
                DelData();
            }
            StartLOL();
            //LoadUsername();
            //LoadUID();
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
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmGetToken_Load(object sender, EventArgs e)
        {
            LoadUsername();
            LoadUID();
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
            /*
            i++;
            if (i == 60) i = 0;
            this.Text = "Get Garena Token by Banana " + i.ToString();*/
            if (IsOpenProc("BsSndRpt"))
            {
                foreach (Process proc in Process.GetProcessesByName("BsSndRpt"))
                {
                    proc.Kill();
                    if (IsOpenProc("RitoBot"))
                    {
                        foreach (Process proc1 in Process.GetProcessesByName("RitoBot"))
                        {
                            proc1.Kill();
                        }
                    }
                }
                StartLOL();
            }
        }

        private void Clock_Tick(object sender, EventArgs e)
        {
            Clock.Enabled = false;
            lblDone.Visible = false;
        }

        #endregion

        #region Func

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
                StreamWriter wr = new StreamWriter(@"config/accounts.txt");
                wr.WriteLine(account);
                wr.Close();
                lblDone.Visible = true;
                Clock.Enabled = true;
            }
        }

        private string Encrypt(string toEncrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);
            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("banana"));
            }
            else keyArray = Encoding.UTF8.GetBytes("banana");
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private string Decrypt(string toDecrypt, bool useHashing)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);
            if (useHashing)
            {
                var hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("banana"));
            }
            else keyArray = Encoding.UTF8.GetBytes("banana");
            var tdes = new TripleDESCryptoServiceProvider
            {
                Key = keyArray,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return Encoding.UTF8.GetString(resultArray);
        }

        private void StartLOL()
        {
            System.Diagnostics.Process.Start("auto.exe");
            CheckProcess("LolClient");
            GetToken();
            foreach (Process proc1 in Process.GetProcessesByName("LolClient"))
            {
                proc1.Kill();
            }
            System.Diagnostics.Process.Start("RitoBot.exe");
            //UpdateProcID();
        }

        private void UpdateProcID()
        {
            CheckProcess("League of Legends");
            foreach (Process proc in Process.GetProcessesByName("League of Legends"))
            {
                procID = proc;
            }
            this.Text = "Get Garena Token by Banana - " + cmbUsername.Text + "(" + procID.Id.ToString() + ")";
        }

        private bool IsOpenProc(String ProcName)
        {
            Process[] processes = Process.GetProcessesByName(ProcName);
            return processes.Length > 0;
        }

        private bool IsOpenProcID(String Name, int Id)
        {
            if (IsOpenProc(Name))
                foreach (Process processes in Process.GetProcessesByName(Name))
                {
                    if (processes.Id == Id) return true;
                }
            return false;
        }

        private void CheckProcess(String ProcessN)
        {
            int Lol = 0;
            while (Lol == 0)
            {
                if (IsOpenProc(ProcessN))
                {
                    Lol = 1;
                }
            }
        }

        #endregion

        #region LoadComboBox
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
        #endregion

    }
}