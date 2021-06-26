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
        DataTable Data = new DataTable();
        public Myinfo()
        {
            InitializeComponent();
            Connect();
            this.Font = new Font(label7.Font.Name, 10, FontStyle.Regular);
        }
        public Myinfo(Color color)
        {
            InitializeComponent();
            Connect();
            label14.BackColor = color;
            this.Font = new Font(label7.Font.Name, 10, FontStyle.Regular);
            panel1.Size = new Size(this.Width, 57);
        }
        IPEndPoint IP;
        Socket client;
        public bool isconnect;
        private void bunifuDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //guna2TextBox1.Text = bunifuDataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            //bunifuDataGridView1.Visible = false;
        }
        int a = 1;
        void Connect()
        {
            //IP là địa chỉ của server.Khởi tạo địa chỉ IP và socket để kết nối
            IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1997);
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //bắt đầu kết nôi. Nếu ko kết nối được thì hiện thông báo
            try
            {
                client.Connect(IP);
                isconnect = client.Connected;
            }
            catch
            {
                MessageBox.Show("Lỗi kết nối", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            //tạo luồng lắng nghe server khi vừa kết nối tới
            Thread listen = new Thread(Receive);
            listen.IsBackground = true;
            listen.Start();
        }
        void close()
        {
            if (isconnect)
                client.Close();
        }
        void Send(data dt)
        {
            client.Send(Serialize(dt));
        }
        void Receive()
        {
            try
            {
                while (true)
                {
                    //khai báo mảng byte để nhận dữ liệu dưới mảng byte
                    byte[] datat = new byte[1024 * 5000];
                    client.Receive(datat);
                    //chuyển data từ dạng byte sang dạng string
                    data dt = (data)Deseriliaze(datat);
                    this.BeginInvoke((MethodInvoker)delegate ()
                    {
                        if (label13.Visible == false)
                            label13.Visible = true;
                    });
                    if (dt.style == 0)
                    {
                        label13.Text = "Account or name has been duplicated!";                       
                    }
                    if (dt.style == 1)
                    {
                        label13.Text = "Email has been duplicated!";
                    }
                    if (dt.style == 2)
                    {
                        panel1.Size = new Size(this.Width, 57);

                        none no = (none)(this.ParentForm);
                        no.changeImage(pictureBox1.Image);
                        Data.Rows[0]["Matkhau"] = Encrypt(Matkhau.Text);
                        Data.Rows[0]["Sex"] = Gt.Text;
                        Data.Rows[0]["Ngaysinh"] = Ns.Text;
                        Data.Rows[0]["Img"] = dt.img;
                        Data.Rows[0]["Ten"] = Ten.Text;
                        Data.Rows[0]["Taikhoan"] = Taikhoan.Text;
                        Data.Rows[0]["Email"] = email.Text;
                        no.recvdata(Data);
                        label13.Text = "Update successful!";
                    }
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
                //close();
            }
        }
        byte[] Serialize(object obj)
        {
            //khởi tạo stream để lưu các byte phân mảnh
            MemoryStream stream = new MemoryStream();
            //khởi tạo đối tượng BinaryFormatter để phân mảnh dữ liệu sang kiểu byte
            BinaryFormatter formatter = new BinaryFormatter();
            //phân mảnh rồi ghi vào stream
            formatter.Serialize(stream, obj);
            //từ stream chuyển các các byte thành dãy rồi cbi gửi đi
            return stream.ToArray();
        }

        //Hàm gom mảnh các byte nhận được rồi chuyển sang kiểu string để hiện thị lên màn hình
        object Deseriliaze(byte[] data)
        {
            //khởi tạo stream đọc kết quả của quá trình phân mảnh 
            MemoryStream stream = new MemoryStream(data);
            //khởi tạo đối tượng chuyển đổi
            BinaryFormatter formatter = new BinaryFormatter();
            //chuyển đổi dữ liệu và lưu lại kết quả 
            return formatter.Deserialize(stream);
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
            email.Text = Data.Rows[0]["Email"].ToString();
            MemoryStream mem = new MemoryStream((byte[])Data.Rows[0]["Img"]);
            pictureBox1.Image =System.Drawing.Image.FromStream(mem);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            Id.ReadOnly = true;
            Taikhoan.ReadOnly = true;
            Matkhau.ReadOnly = true;
            Ten.ReadOnly = true;
            Gt.ReadOnly = true;
            Ns.ReadOnly = true;
            Nt.ReadOnly = true;
            email.ReadOnly = true;
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
                if (label13.Visible==true)
                    label13.Visible = false;
                if ((int)checkpass(Matkhau.Text) < 3)
                {
                    label12.Text = "Password is too weak";
                    label12.Visible = true;
                }
                else
                {
                    if (jpegByteSize < 1000000)
                    {
                        if (IsValidEmail(email.Text))
                        {
                            try
                            {

                                data dt = new data();
                                string ten = Ten.Text;
                                string tk = Taikhoan.Text;
                                string mk = Encrypt(Matkhau.Text);
                                string gt = Gt.Text;
                                string ngaysinh = Ns.Text;
                                string Email = email.Text;
                                dt.ten = ten;
                                dt.email = Email;
                                dt.tk = tk;
                                dt.mk = mk;
                                dt.id = Id_M;
                                dt.ngaysinh = ngaysinh;
                                dt.sex = gt;
                                MemoryStream mem = new MemoryStream();
                                pictureBox1.Image.Save(mem, pictureBox1.Image.RawFormat);
                                dt.img = mem.ToArray();
                                dt.style = 5;
                                Send(dt);
                                
                            }
                            catch { }
                        }
                        else
                        {
                            label13.Text = "This email is not valid!";
                            if (label13.Visible == false)
                                label13.Visible = true;
                        }    
                    }
                    else
                    {
                        label13.Text = "The image size is too large";
                        if (label13.Visible == false)
                            label13.Visible = true;
                    }
                }
            }
            else
            {
                label13.Text = "Please fill it out completely";
                if (label13.Visible == false)
                    label13.Visible = true;
            }
        }
        public void RId(string id)
        {
            Id_M = id;
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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

        private void label7_MouseHover(object sender, EventArgs e)
        {
            label7.Font = new Font(this.Font,FontStyle.Underline);
        }

        private void label7_MouseLeave(object sender, EventArgs e)
        {
            label7.Font = new Font(this.Font, FontStyle.Regular);
        }

        private void label8_MouseHover(object sender, EventArgs e)
        {
            label8.Font = new Font(this.Font, FontStyle.Underline);

        }

        private void label8_MouseLeave(object sender, EventArgs e)
        {
            label8.Font = new Font(this.Font, FontStyle.Regular);

        }

        private void label9_MouseHover(object sender, EventArgs e)
        {
            label9.Font = new Font(this.Font, FontStyle.Underline);

        }

        private void label9_MouseLeave(object sender, EventArgs e)
        {
            label9.Font = new Font(this.Font, FontStyle.Regular);


        }

        private void label10_MouseHover(object sender, EventArgs e)
        {
            label10.Font = new Font(this.Font, FontStyle.Underline);

        }

        private void label10_MouseLeave(object sender, EventArgs e)
        {
            label10.Font = new Font(this.Font, FontStyle.Regular);

        }

        private void label11_MouseHover(object sender, EventArgs e)
        {
            label11.Font = new Font(this.Font, FontStyle.Underline);

        }

        private void label11_MouseLeave(object sender, EventArgs e)
        {
            label11.Font = new Font(this.Font, FontStyle.Regular);

        }

        private void label16_MouseHover(object sender, EventArgs e)
        {
            label16.Font = new Font(this.Font, FontStyle.Underline);
        }

        private void label16_MouseLeave(object sender, EventArgs e)
        {
            label16.Font = new Font(this.Font, FontStyle.Regular);
        }

        private void label16_Click(object sender, EventArgs e)
        {
            email.ReadOnly = false;
        }
    }
}
