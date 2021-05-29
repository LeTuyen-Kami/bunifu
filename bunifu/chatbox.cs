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

namespace bunifu
{
    public partial class chatbox : UserControl
    {
        ClienT clt=new ClienT();
        string Id_M = "";
        string Id_N = "";
        public chatbox()
        {
            if (!this.DesignMode)
            {
                InitializeComponent();
            }
            bb_old.Top = 0 - bb_old.Height + 10;
        }

        buble bb_old = new buble();
        public void addinmessage(string message,string time,buble.msgtype a)
        {
            buble bb1 = new buble(message, time, a);
            bb1.Location = buble1.Location;
            bb1.Size = buble1.Size;
            bb1.Anchor = buble1.Anchor;
            bb1.Top = bb_old.Bottom + 10;
            panel1.Controls.Add(bb1);
            if (a==buble.msgtype.Out)
            {
                bb1.Left += panel1.Width - bb1.Width - 40;

            }
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            bb_old = bb1;
        }
        /*public void addoutmessage(string message, string time)
        {
            buble bb1 = new buble(message, time, buble.msgtype.Out);
            bb1.Location = buble1.Location;
            bb1.Size = buble1.Size;
            bb1.Anchor = buble1.Anchor;
            bb1.Top = bb_old.Bottom + 10;
            panel1.Controls.Add(bb1);
            bb1.Left += panel1.Width - bb1.Width-40;
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            bb_old = bb1;
        }*/
        private void chatbox_Load(object sender, EventArgs e)
        {
            bunifuImageButton1.Enabled = false;
            bunifuImageButton1.BringToFront();
            clt.Connect();
        }
        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            if (textBox1.Text != "")
                bunifuImageButton1.Enabled = true;
            else
                bunifuImageButton1.Enabled = false;
        }
        public void vertical()
        {
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
        }
        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            string s = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
            addinmessage(textBox1.Text, s,buble.msgtype.Out);
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;

            data dt = new data();
            dt.msg =Encrypt(textBox1.Text);
            dt.id_recv = Int32.Parse(Id_N);
            dt.id_send = Int32.Parse(Id_M);
            dt.time = s;
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
    }
 
}
