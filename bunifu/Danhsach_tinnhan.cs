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
using System.IO;

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
        byte[] my_image;
        string Id_B;
        string My_name;
        public Danhsach_tinnhan()
        {
            InitializeComponent();
        }
        public Danhsach_tinnhan(Color color)
        {
            InitializeComponent();
            label1.BackColor = color;
        }
        public void nhap_data(DataTable dataTable_ac,DataTable dataTable_nhom,string id,byte[] pic,string name)
        {
            My_name = name;
            my_image = pic;
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
            panel5.Controls.Add(panel);
            Id_M = id;
            datat = dataTable_ac;
            data_nhom = dataTable_nhom;
            foreach (DataRow row in dataTable_ac.Rows)
            {
                byte[] img = new byte[1024*4000];
                string s = row["Ten"].ToString();
                string Id = row["Id"].ToString();
                string Trangthai = row["Trangthai"].ToString();
                if(!row.IsNull("Img"))
                {
                    img = (byte[])row["Img"];
                }
                if (s != "")
                {
                    Banbe banbe = new Banbe();
                    banbe.Addten(s, Id, img,Id_M,0,Trangthai);
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
        public void showchatbox(string s,byte[] img,string ten,string loai_mes,string trangthai,string Ten_nhom)
        {
            DataSet ds = new DataSet();
            Id_B = s;
            int width = panel3.Width;
            chat = new chatbox();
            chat.Dock = DockStyle.Fill;
            chat.Thongtin(ten,img,my_image,loai_mes,My_name,datat,trangthai,Ten_nhom);
            chat.IdM(Id_M);
            chat.IdN(s);
            Create_Connect();
            string sqll="";
            if (loai_mes=="0")
            {
                sqll = "select * from Message where Id_S = '" + Id_M + "' and Id_R = '" + s + "' or Id_S = '" + s + "' and Id_R = '" + Id_M + "'";
            }
            if (loai_mes=="1")
            {
                sqll = "select Message.*,Account.Ten,Account.Img " +
                    "from Message, Account " +
                    "where Id_R = '"+s+"' and Loai_mes = '1' and Message.Id_S = Account.Id";
            }
            SqlDataAdapter adapter = new SqlDataAdapter(sqll, strConnect);
            adapter.Fill(ds, "Message");
            DataTable dt_mes = ds.Tables["Message"];
            foreach (DataRow row in dt_mes.Rows)
            {
                string message = Decrypt(row["Message"].ToString());
                string time = row["Time"].ToString();
                string id = row["Id_S"].ToString();
                string loainhan = row["Loai_nhan"].ToString();
                byte[] image_send = new byte[1];
                if (loainhan=="1")
                {
                    image_send = (byte[])row["Data_byte"];
                    
                }    
                byte[] pic = img;
                string Ten_mes = "";
                if (loai_mes=="1")
                {
                    Ten_mes = row["Ten"].ToString();
                    string[] source = Ten_mes.Split(new char[] { ' ' });
                    Ten_mes = source[source.Count() - 1];
                    pic = (byte[])row["Img"];
                }
                if (id.ToString() == Id_M)
                {
                    chat.addinmessage(message, time, buble.msgtype.Out, width, pic,Ten_mes,loainhan,image_send);
                }
                else
                    chat.addinmessage(message, time, buble.msgtype.In, width, pic,Ten_mes,loainhan,image_send);
            }    
            strConnect.Close();
            panel3.Controls.Add(chat);
            chat.vertical();
            chat.BringToFront();
        }
        public void add_mes(data dt,int loai)
        {
            int width = panel3.Width;
            string s = dt.msg;
            byte[] img = dt.img;
            string name = dt.ten;
            string time = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();           
            s = Decrypt(s);
            string Id_chatbox;
            if (loai == 0)
                Id_chatbox = dt.id_send.ToString();
            else
                Id_chatbox = dt.id_recv.ToString();
            if (Id_chatbox == Id_B && dt.id_send.ToString() != Id_M)
            {
                chat.addinmessage(s, time, buble.msgtype.In, width, img,name,"0",null);
            }

        }
        private void guna2Button5_Click(object sender, EventArgs e)
        {
            
        }
        public static string Decrypt(string toDecrypt)
        {
            if (toDecrypt == "")
                return toDecrypt;
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
                label2.Text = "Group";
                Panel panel = new Panel();
                panel5.Controls.Add(panel);
                panel.Dock = DockStyle.Fill;
                panel.AutoScroll = true;
                panel.BringToFront();
                foreach (DataRow row in data_nhom.Rows)
                {
                    string s = row["Tennhom"].ToString();
                    string Id = row["Id_nhom"].ToString();
                    byte[] img = new byte[100];
                    Banbe banbe = new Banbe();
                    banbe.Addten(s, Id, img, Id_M, 2,"");
                    banbe.usercontrol(this);
                    banbe.Dock = DockStyle.Top;
                    panel.Controls.Add(banbe);

                }
            }   
            else
            {
                label2.Text = "Friend";
                Panel panel = new Panel();
                panel5.Controls.Add(panel);
                panel.AutoScroll = true;
                panel.BringToFront();
                panel.Dock = DockStyle.Fill;
                foreach (DataRow row in datat.Rows)
                {
                    byte[] img = new byte[1024 * 4000];
                    string s = row["Ten"].ToString();
                    string Id = row["Id"].ToString();
                    string Trangthai = row["Trangthai"].ToString();
                    if (row["Img"] != null)
                    {
                        img = (byte[])row["Img"];
                    }
                    if (s != "")
                    {
                        Banbe banbe = new Banbe();
                        banbe.Addten(s, Id, img, Id_M, 0,Trangthai);
                        banbe.usercontrol(this);
                        banbe.Dock = DockStyle.Top;
                        panel.Controls.Add(banbe);
                    }
                }
            }    
        }
    }
}
