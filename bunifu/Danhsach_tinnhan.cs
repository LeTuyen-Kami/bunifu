using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace bunifu
{
    public partial class Danhsach_tinnhan : UserControl
    {
        chatbox chat;
        string Id_M;
        DataTable datat = new DataTable();
        DataTable data_nhom = new DataTable();
        DateTime now = DateTime.Now;
        SqlConnection strConnect = new SqlConnection();

        public Danhsach_tinnhan()
        {
            InitializeComponent();
        }
        public void nhap_data(DataTable dataTable_ac,DataTable dataTable_nhom,string id)
        {
            Panel panel = new Panel();
            panel5.Controls.Add(panel);
            Id_M = id;
            datat = dataTable_ac;
            data_nhom = dataTable_nhom;
            foreach (DataRow row in dataTable_ac.Rows)
            {
                byte[] img = new byte[1024*4000];
                string s = row["Ten"].ToString();
                string Id = row["Id"].ToString();
                if(row["Img"]!=null)
                {
                    img = (byte[])row["Img"];
                }
                if (s != "")
                {
                    Banbe banbe = new Banbe();
                    banbe.Addten(s, Id, img,Id_M,0);
                    banbe.usercontrol(this);
                    banbe.Dock = DockStyle.Top;
                    panel.Controls.Add(banbe);
                }
            }
        }
        public void Create_Connect()
        {
            string str = "Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True";
            strConnect.ConnectionString = str;
            strConnect.Open();
        }
        public void showchatbox(string s,byte[] img,string ten)
        {
            int width = panel3.Width;
            chat = new chatbox();
            chat.Dock = DockStyle.Fill;
            chat.Thongtin(ten,img);
            chat.IdM(Id_M);
            chat.IdN(s);
            Create_Connect();
            string sqll = "select * from Message where Id_S = '" + Id_M + "' and Id_R = '" + s + "' or Id_S = '" + s + "' and Id_R = '" + Id_M + "'";
            SqlCommand cmd = new SqlCommand(sqll, strConnect);
            SqlDataReader dta = cmd.ExecuteReader();
            if (dta.HasRows)
            {
                while (dta.Read())
                {
                    string message = Decrypt(dta.GetString(3));
                    string time = dta.GetString(4);
                    int id = dta.GetInt32(1);
                    if (id.ToString() == Id_M)
                    {
                        chat.addinmessage(message, time, buble.msgtype.Out,width);
                    }
                    else
                        chat.addinmessage(message, time, buble.msgtype.In,width);
                }
            }
            strConnect.Close();
            panel3.Controls.Add(chat);
            chat.vertical();
            chat.BringToFront();
        }
        public void add_mes(string s)
        {
            int width = panel3.Width;
            string time = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();           
            s = Decrypt(s);
            chat.addinmessage(s, time, buble.msgtype.In,width);
        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            
        }
        public static string Decrypt(string toDecrypt)
        {
            string key = "Kamisama43423";
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            if (useHashing)
            {
                MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            }
            else
                keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return UTF8Encoding.UTF8.GetString(resultArray);
        }

        private void guna2ToggleSwitch1_CheckedChanged(object sender, EventArgs e)
        {
            if  (guna2ToggleSwitch1.Checked==true)
            {
                Panel panel = new Panel();
                panel5.Controls.Add(panel);
                foreach (DataRow row in data_nhom.Rows)
                {
                    string s = row["Ten"].ToString();
                    string Id = row["Id"].ToString();
                    if (s != "")
                    {
                        byte[] img = new byte[100];
                        Banbe banbe = new Banbe();
                        banbe.Addten(s, Id, img, Id_M, 2);
                        banbe.usercontrol(this);
                        banbe.Dock = DockStyle.Top;
                        panel.Controls.Add(banbe);
                    }
                }
            }   
            else
            {
                Panel panel = new Panel();
                panel5.Controls.Add(panel);
                foreach (DataRow row in datat.Rows)
                {
                    byte[] img = new byte[1024 * 4000];
                    string s = row["Ten"].ToString();
                    string Id = row["Id"].ToString();
                    if (row["Img"] != null)
                    {
                        img = (byte[])row["Img"];
                    }
                    if (s != "")
                    {
                        Banbe banbe = new Banbe();
                        banbe.Addten(s, Id, img, Id_M, 0);
                        banbe.usercontrol(this);
                        banbe.Dock = DockStyle.Top;
                        panel.Controls.Add(banbe);
                    }
                }
            }    
        }
    }
}
