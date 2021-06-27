using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace bunifu
{
    public partial class Signup : UserControl
    {
        IPEndPoint IP;
        Socket client;
        public bool isconnect;
        public Signup()
        {
            InitializeComponent();
        }
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
                    string dt = (string)Deseriliaze(datat);
                    if (this.InvokeRequired)
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            if (dt == "0")
                            {
                                MessageBox.Show("Tạo tài khoản thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Account, name has been duplicated", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        });

                }
            }
            catch
            {
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
        private void bunifuTextBox3_TextChange(object sender, EventArgs e)
        {
            if (bunifuTextBox3.Text != "")
            {
                label3.Text = checkpass(bunifuTextBox3.Text).ToString();
                label3.Visible = true;
            }
            else
                label3.Visible = false;
        }

        private void bunifuTextBox4_TextChange(object sender, EventArgs e)
        {
            if (bunifuTextBox4.Text != "")
            {
                label4.Text = checkpass(bunifuTextBox4.Text).ToString();
                label4.Visible = true;
            }
            else
                label4.Visible = false;
        }
        private void bunifuTextBox1_TextChange(object sender, EventArgs e)
        {
            label5.Visible = false;
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {         
            if (bunifuTextBox1.Text!=""&bunifuTextBox2.Text!=""&&bunifuTextBox3.Text!=""&&bunifuTextBox4.Text!="")
            {
                if (bunifuTextBox3.Text != bunifuTextBox4.Text)
                {
                    label5.Text = "Password does not match!";
                    label5.Visible = true;
                }
                else
                {
                    if ((int)checkpass(bunifuTextBox3.Text) < 3)
                    {
                        label5.Text = "Password is too weak";
                        label5.Visible = true;
                    }
                    else
                    {
                        try
                        {
                            Connect();
                            data dt = new data();
                            string ten = bunifuTextBox1.Text;
                            string tk = bunifuTextBox2.Text;
                            string mk = Encrypt(bunifuTextBox3.Text);
                            dt.ten = ten;
                            dt.img = File.ReadAllBytes(@"D:\Lập trình mạng\bunifu\user.png");
                            dt.tk = tk;
                            dt.mk = mk;
                            dt.style = 2;
                            Send(dt);
                        }
                        catch { }
                        Login login = (Login)(this.ParentForm);
                        login.click();
                        bunifuTextBox1.Text = "";
                        bunifuTextBox2.Text = "";
                        bunifuTextBox3.Text = "";
                        bunifuTextBox4.Text = "";
                    }
                }              
            }    
            else
            {
                label5.Text = "Please fill it out completely";
                label5.Visible = true;
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
    }
}
