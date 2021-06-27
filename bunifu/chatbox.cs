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
        string TenNhom;
        DataTable dt_friend;
        DataTable dt_send=new DataTable();
        int point;
        int height=0;
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
            if (loainhan=="0"&&message!="")
            {
               bb1 = new buble(message, time, a);
            }
            if (message==""&&loainhan=="0")
            {
                bb1 = new buble(1);
            }    
            if (loainhan=="1")
            {
                MemoryStream mem = new MemoryStream(img);
                Image image1 = System.Drawing.Image.FromStream(mem);
                bb1 = new buble(image1);
            }
            if (loainhan=="2"&&message!="")
            {
                bb1 = new buble(message,time, img,a);
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
            dt_send.Columns.Add("Ten");
            dt_send.Columns.Add("Id");
  
            //bunifuImageButton1.Enabled = false;
            bunifuImageButton1.BringToFront();
            clt.Connect();
            guna2DataGridView1.BringToFront();
            panel1.Controls.Add(guna2DataGridView1);
            point = panel1.Bottom;
            height = guna2DataGridView1.Height;
        }
        public void Thongtin(string s,byte[] img,byte[] my_pic,string loai_mes,string name,DataTable data_friend,string trangthai,string Ten_nhom)
        {
            TenNhom = Ten_nhom;
            dt_friend = data_friend;
            dt = new DataTable();
            dt.Columns.Add("Ten");
            dt_id.Columns.Add("Id");
            foreach (DataRow row in data_friend.Rows)
            {
                dt.Rows.Add(row["Ten"]);
                dt_id.Rows.Add(row["Id"]);
            }    
            My_name = name;
            loaimes = loai_mes;
            my_img = my_pic;
            if (loai_mes=="1")
            {
                guna2CirclePictureBox1.Visible = false;
                guna2CirclePictureBox2.Visible = false;
            }
            if (loai_mes!="1")
            {
                MemoryStream mem = new MemoryStream(img);
                guna2CirclePictureBox1.Image =System.Drawing.Image.FromStream(mem);
                guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                if (trangthai=="off")
                {
                    guna2CirclePictureBox2.FillColor = Color.Gray;
                }    
                if (trangthai=="on")
                {
                    guna2CirclePictureBox2.FillColor = Color.Green;
                }    
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
            addinmessage(richTextBox1.Text, s, buble.msgtype.Out, left_mes, new byte[100], "", "0", null);
            panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
            DataTable tam = new DataTable("Tag");
            tam.Columns.Add("Ten");
            tam.Columns.Add("Id");
            data dt = new data();
            dt.ten = My_name;
            dt.tk = TenNhom;
            dt.msg = Encrypt(richTextBox1.Text);
            dt.id_recv = Int32.Parse(Id_N);
            dt.id_send = Int32.Parse(Id_M);
            dt.time = s;
            if (dt_send.Rows.Count > 0)
            {
                foreach (DataRow row in dt_send.Rows)
                {
                    if (richTextBox1.Text.Contains(row["Ten"].ToString()))
                        tam.Rows.Add(new object[] { row["Ten"], row["Id"] });
                }
                dt.ds.Tables.Add(tam);
            }
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
                bunifuImageButton1.Image = new Bitmap(@"send.png");
            else
            {
                bunifuImageButton1.Image = new Bitmap(@"like.png");
            }    
            if (richTextBox1.Text.Contains("@")&&loaimes=="1")
            {
                if (b > 0)
                {
                    richTextBox1.Select(vitri + 1, b);
                    string s = richTextBox1.SelectedText;
                    dt = new DataTable();
                    dt_id = new DataTable();
                    dt_id.Columns.Add("Id");
                    dt.Columns.Add("Ten");
                    foreach (DataRow row in dt_friend.Rows)
                    {
                        if (row["Ten"].ToString().ToLower().StartsWith(s))
                        {
                            dt.Rows.Add(row["Ten"].ToString());
                            dt_id.Rows.Add(row["Id"].ToString());
                        }
                    }
                    guna2DataGridView1.DataSource = dt;
                    richTextBox1.Select(vitri + 1 + b, 0);
                    guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                        (guna2DataGridView1.RowCount) * guna2DataGridView1.RowTemplate.Height);
                    if (bb_old.Bottom >point)
                    {
                        guna2DataGridView1.Top = bb_old.Bottom;
                    }
                    else
                    {
                        guna2DataGridView1.Location = new Point(guna2DataGridView1.Location.X, panel1.Bottom - guna2DataGridView1.Height);
                    }    
                    guna2DataGridView1.Visible = true;
                    panel1.VerticalScroll.Value = panel1.VerticalScroll.Maximum;
                }
            }
            else
            {
                guna2DataGridView1.Visible = false;
            }
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

        private void richTextBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            dem();
            if (e.KeyChar == 64&&loaimes=="1")
            {
                //check_ = 1;
                b = 0;
                vitri = richTextBox1.Text.Count();              
                guna2DataGridView1.Visible = true;
                guna2DataGridView1.DataSource = dt;
                guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                            guna2DataGridView1.RowCount * guna2DataGridView1.RowTemplate.Height);
                if (bb_old.Bottom > point + height)
                {
                    guna2DataGridView1.Top = bb_old.Bottom;
                }
                else
                {
                    guna2DataGridView1.Location = new Point(guna2DataGridView1.Location.X, panel1.Bottom - guna2DataGridView1.Height);
                }
            }
            if (e.KeyChar == 8&&loaimes=="1")
            {
                if ((richTextBox1.SelectionStart > vitri && richTextBox1.SelectionStart < vitri_cuoi - 1) && check_ == 1)
                {
                    richTextBox1.Select(vitri, vitri_cuoi - 1 - vitri);
                    richTextBox1.SelectedText = "";
                    richTextBox1.Select(vitri, 1);
                    richTextBox1.SelectionBackColor = Color.White;
                    check_ = 0;
                }
            }
        }
        int b = 0;
        void dem()
        {
            b++;
        }
        int vitri_cuoi = 0; 
        int check_ = 0;
        int vitri = 0;
        DataTable dt = new DataTable();
        DataTable dt_id=new DataTable();
        private void guna2DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            check_ = 1;
            richTextBox1.Select(vitri, b + 1);
            richTextBox1.SelectedText = "";
            string ten = guna2DataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            string id = dt_id.Rows[e.RowIndex]["Id"].ToString();
            richTextBox1.Text += ten;
            richTextBox1.SelectionStart = vitri;
            richTextBox1.SelectionLength = guna2DataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Count();
            vitri_cuoi = vitri + guna2DataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString().Count() + 1;
            richTextBox1.SelectionBackColor = Color.LightBlue;
            richTextBox1.Select(vitri_cuoi, 0);
            richTextBox1.SelectionBackColor = Color.White;
            guna2DataGridView1.Visible = false;
            dt_send.Rows.Add(new object[] {ten,id });
        }

        private void panel1_DragDrop(object sender, DragEventArgs e)
        {
            string[] dropfile = (string[])e.Data.GetData(DataFormats.FileDrop);
            DateTime now = DateTime.Now;
            string s = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
            byte[] tam = File.ReadAllBytes(dropfile[0]);
            string file_name = Path.GetFileNameWithoutExtension(dropfile[0]);
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

        private void panel1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }
    }
 
}
