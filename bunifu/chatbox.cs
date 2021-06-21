using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;
using Guna.UI2.WinForms;

namespace bunifu
{
    public partial class chatbox : UserControl
    {
        ClienT clt=new ClienT();
        string Id_M = "";
        string Id_N = "";
        int left_mes;
        string loaimes;
        string My_name;
        byte[] my_img;
        string path_image;
        public chatbox()
        {
            if (!this.DesignMode)
            {
                InitializeComponent();
            }
            bb_old.Top = 0 - bb_old.Height + 10;
        }

        buble bb_old = new buble();
        public void addinmessage(string message, string time, buble.msgtype a, int width, byte[] image, string Ten_mes, string loainhan,byte[] img)
        {
            left_mes = width;
            buble bb1=new buble();
            if (loainhan=="0")
            {
               bb1 = new buble(message, time, a);
            }
            if (loainhan=="1")
            {
                MemoryStream mem = new MemoryStream(img);
                Image image1 = System.Drawing.Image.FromStream(mem);
                bb1 = new buble(image1);
            }
            if (loainhan=="2")
            {
                bb1 = new buble(message,time, img);
            }    
            bb1.Location = buble1.Location;
            //bb1.Size = buble1.Size;
            bb1.Anchor = buble1.Anchor;
            bb1.Top = bb_old.Bottom + 20;

            panel1.Controls.Add(bb1);
            if (a == buble.msgtype.Out)
            {
                bb1.Left = width - bb1.Width - 40;
            }
            if (a == buble.msgtype.In)
            {
                Guna2CirclePictureBox pic = new Guna2CirclePictureBox();
                pic.Size = new Size(30, 30);
                pic.Location = new Point(6, bb1.Bottom - 30);
                pic.BringToFront();
                MemoryStream mem = new MemoryStream(image);
                pic.Image = System.Drawing.Image.FromStream(mem);
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                panel1.Controls.Add(pic);
                if (loaimes == "1")
                {
                    Label lb_name = new Label();
                    lb_name.AutoSize = true;
                    lb_name.Font = new Font(lb_name.Font.Name, 6, lb_name.Font.Style);
                    lb_name.Location = new Point(bb1.Location.X, bb1.Location.Y - 10);
                    lb_name.Text = Ten_mes;
                    lb_name.BringToFront();
                    panel1.Controls.Add(lb_name);
                }
            }
            bb_old = bb1;
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
        }
        private void chatbox_Load(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            bunifuImageButton1.BringToFront();
            clt.Connect();

        }
        public void Thongtin(string s,byte[] img,byte[] my_pic,string loai_mes,string name)
        {
            My_name = name;
            loaimes = loai_mes;
            my_img = my_pic;
            if (loai_mes=="1")
            {
                guna2CirclePictureBox1.Visible = false;
            }    
            if (loai_mes!="1")
            {
                MemoryStream mem = new MemoryStream(img);
                guna2CirclePictureBox1.Image =System.Drawing.Image.FromStream(mem);
                guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            label1.Text = s;
        }
        public void vertical()
        {
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
        }
        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string s = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
            addinmessage(richTextBox1.Text, s,buble.msgtype.Out,left_mes,new byte[100],"","0",null);
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;

            data dt = new data();
            dt.ten = My_name;
            dt.msg =Encrypt(richTextBox1.Text);
            dt.id_recv = Int32.Parse(Id_N);
            dt.id_send = Int32.Parse(Id_M);
            dt.time = s;
            dt.img = my_img;
            dt.loai_mes = loaimes;
            dt.loai_nhan = "0";
            clt.Send(dt);
        }
        public void IdM(string id)
        {
            Id_M = id;
        }
        public void IdN(string id)
        {
            Id_N = id;
        }
        public static string Encrypt(string toEncrypt)
        {
            if (toEncrypt == "")
                return toEncrypt;
            string key = "Kamisama43423";
            bool useHashing = true;
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

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

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        private void panel1_SizeChanged(object sender, EventArgs e)
        {

        }
        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            if (richTextBox1.Text != "")
                bunifuImageButton1.Enabled = true;
            else
                bunifuImageButton1.Enabled = false;
        }

        private void guna2ImageButton2_Click(object sender, EventArgs e)
        {
            byte[] tam=new byte[1];
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp;*.png)|*.jpg; *.jpeg; *.gif; *.bmp;*.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    Image img = new Bitmap(open.FileName);
                    tam = File.ReadAllBytes(open.FileName);
                    
                    
                }
            }
            if (tam.Length < 10000000)
            {
                addinmessage(richTextBox1.Text, "", buble.msgtype.Out, left_mes, new byte[100], "", "1", tam);
                data dt = new data();
                dt.ten = My_name;
                dt.id_recv = Int32.Parse(Id_N);
                dt.id_send = Int32.Parse(Id_M);
                dt.img = my_img;
                dt.image = tam;
                dt.loai_mes = loaimes;
                dt.loai_nhan = "1";
                clt.Send(dt);
            }
            else
                MessageBox.Show("your file size is too big (10Mb)", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string s = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
            byte[] tam = new byte[1];
            string file_name="";
            using (OpenFileDialog open = new OpenFileDialog())
            {
                if (open.ShowDialog() == DialogResult.OK)
                {
                    file_name = open.SafeFileName;
                    tam = File.ReadAllBytes(open.FileName);
                }
            }
            if (tam.Length < 10000000)
            {
                addinmessage(file_name, s, buble.msgtype.Out, left_mes, new byte[100], "", "2", tam);
                data dt = new data();
                dt.ten = My_name;
                dt.msg = Encrypt(file_name);
                dt.id_recv = Int32.Parse(Id_N);
                dt.id_send = Int32.Parse(Id_M);
                dt.img = my_img;
                dt.time = s;
                dt.image = tam;
                dt.loai_mes = loaimes;
                dt.loai_nhan = "2";
                clt.Send(dt);
            }
            else
                MessageBox.Show("your file size is too big (10Mb)", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
 
}
