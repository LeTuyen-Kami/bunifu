using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace bunifu
{
    public partial class Myinfo : UserControl
    {
        ClienT clienT = new ClienT();
        DataTable Data = new DataTable();
        public Myinfo()
        {
            InitializeComponent();
            clienT.Connect();
        }
        public Myinfo(Color color)
        {
            InitializeComponent();
            clienT.Connect();
            label14.BackColor = color;
        }
        string Id_M;      
        int temp = 0;

        private void label3_Click(object sender, EventArgs e)
        {
            if (temp % 2 == 0)
            {
                Matkhau.UseSystemPasswordChar = false;
            }
            else
            {
                Matkhau.UseSystemPasswordChar = true;
            }
            temp++;
        }

        private void Myinfo_Load(object sender, EventArgs e)
        {
            
        }
        public void datatable(DataTable dataTable)
        {
            Data = dataTable;
            Id.Text = Data.Rows[0][0].ToString();
            Taikhoan.Text = Data.Rows[0][1].ToString();
            Matkhau.Text = Decrypt(Data.Rows[0][2].ToString());
            Ten.Text = Data.Rows[0][3].ToString();
            Gt.Text = Data.Rows[0][5].ToString();
            Ns.Text = Data.Rows[0][6].ToString();
            Nt.Text = Data.Rows[0][7].ToString();
            MemoryStream mem = new MemoryStream((byte[])Data.Rows[0][4]);
            pictureBox1.Image =System.Drawing.Image.FromStream(mem);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Id.ReadOnly = true;
            Taikhoan.ReadOnly = true;
            Matkhau.ReadOnly = true;
            Ten.ReadOnly = true;
            Gt.ReadOnly = true;
            Ns.ReadOnly = true;
            Nt.ReadOnly = true;
        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.Filter = "Image Files(*.jpg; *.jpeg; *.gif; *.bmp)|*.jpg; *.jpeg; *.gif; *.bmp;*.png";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    // display image in picture box  
                    pictureBox1.Image = new Bitmap(open.FileName);
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }                          
        }
        public enum PasswordScore
        {
            Blank = 0,
            VeryWeak = 1,
            Weak = 2,
            Medium = 3,
            Strong = 4,
            VeryStrong = 5
        }
        public PasswordScore checkpass(string password)
        {
            int score = 0;

            if (password.Length < 1)
                return PasswordScore.Blank;
            if (password.Length < 4)
                return PasswordScore.VeryWeak;

            if (password.Length >= 6)
                score++;
            if (password.Length >= 8)
                score++;
            if (Regex.Match(password, @"\d+", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @"[a-z]", RegexOptions.ECMAScript).Success &&
              Regex.Match(password, @"[A-Z]", RegexOptions.ECMAScript).Success)
                score++;
            if (Regex.Match(password, @".[!,@,#,$,%,^,&,*,?,_,~,-,£,(,)]", RegexOptions.ECMAScript).Success)
                score++;

            return (PasswordScore)score;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            long jpegByteSize;
            using (var ms = new MemoryStream()) // estimatedLength can be original fileLength
            {
                pictureBox1.Image.Save(ms, pictureBox1.Image.RawFormat); // save image to stream in Jpeg format
                jpegByteSize = ms.Length;
            }
            if (Ten.Text != "" && Taikhoan.Text != "" && Matkhau.Text != "")
            {
                if ((int)checkpass(Matkhau.Text) < 3)
                {
                    label12.Text = "Password is too weak";
                    label12.Visible = true;
                }
                else
                {
                    if (jpegByteSize < 1000000)
                    {
                        try
                        {
                            
                            data dt = new data();
                            string ten = Ten.Text;
                            string tk = Taikhoan.Text;
                            string mk = Encrypt(Matkhau.Text);
                            string gt = Gt.Text;
                            string ngaysinh = Ns.Text;
                            dt.ten = ten;
                            dt.tk = tk;
                            dt.mk = mk;
                            dt.id = Id_M;
                            dt.ngaysinh = ngaysinh;
                            dt.sex = gt;
                            MemoryStream mem = new MemoryStream();
                            pictureBox1.Image.Save(mem, pictureBox1.Image.RawFormat);
                            dt.img = mem.ToArray();                                            
                            dt.style = 5;
                            clienT.Send(dt);
                            none no = (none)(this.ParentForm);
                            no.changeImage(pictureBox1.Image);
                            Data.Rows[0]["Ten"] = Ten.Text;
                            Data.Rows[0]["Taikhoan"] = Taikhoan.Text;
                            Data.Rows[0]["Matkhau"] = Encrypt(Matkhau.Text);
                            Data.Rows[0]["Sex"] = Gt.Text;
                            Data.Rows[0]["Ngaysinh"] = Ns.Text;
                            Data.Rows[0]["Img"] = dt.img;
                            no.recvdata(Data);
                        }
                        catch { }
                    }
                    else
                    {
                        label13.Text = "Hình ảnh có dung lượng quá lớn";
                        label13.Visible = true;
                    }
                }
            }
            else
            {
                label13.Text = "Please fill it out completely";
                label13.Visible = true;
            }
        }
        public void RId(string id)
        {
            Id_M = id;
        }
        public static string Encrypt(string toEncrypt)
        {
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

        private void label7_Click(object sender, EventArgs e)
        {
            Ten.ReadOnly = false;
        }

        private void label8_Click(object sender, EventArgs e)
        {
            Taikhoan.ReadOnly = false;

        }

        private void label9_Click(object sender, EventArgs e)
        {
            Matkhau.ReadOnly = false;

        }

        private void label10_Click(object sender, EventArgs e)
        {
            Gt.ReadOnly = false;

        }

        private void label11_Click(object sender, EventArgs e)
        {
            Ns.ReadOnly = false;

        }
        int dem = 0;

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {
            if (Matkhau.Text != "" && dem != 0)
            {
                label12.Text = checkpass(Matkhau.Text).ToString();
                label12.Visible = true;
            }
            else
                label12.Visible = false;
            dem = 1;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            none no = (none)(this.ParentForm);
            no.Close();
        }
    }
}
