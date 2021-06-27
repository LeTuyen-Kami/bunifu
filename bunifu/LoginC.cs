using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace bunifu
{
    public partial class LoginC : UserControl
    {
        public LoginC()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            string path = @"tk_mk.txt";
            if (File.Exists(path))
            {
                using (FileStream fs = File.OpenRead(path))
                {
                    StreamReader sd = new StreamReader(fs);
                    string ct = sd.ReadToEnd();
                    string[] content = ct.Split('\n');
                    guna2TextBox1.Text = content[0];
                    guna2TextBox2.Text = content[1];
                    if (content[2] == "0")
                    {
                        guna2CheckBox1.Checked = false;
                    }
                    else
                        guna2CheckBox1.Checked = true;

                }
            }            
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
                    string dt = (string)Deseriliaze(datat);
                    
                    if (this.InvokeRequired)
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            if (dt != "0")
                            {
                                none no = new none();
                                no.Id(dt);
                                no.Show();
                                Login login = (Login)(this.ParentForm);
                                login.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Tài khoản,mật khẩu không chính xác", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                guna2Button1.Enabled = true;
                                guna2TextBox1.Text = "";
                                guna2TextBox2.Text = "";
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
        private void LoginC_Load(object sender, EventArgs e)
        {
            //bunifuDataGridView1.Visible = false;
            guna2TextBox2.UseSystemPasswordChar = true;
        }
        private void bunifuImageButton1_Click_1(object sender, EventArgs e)
        {

            if (a == 1)
            {
                a = 2;
                guna2TextBox2.UseSystemPasswordChar = false;
            }
            else
            {
                a = 1;
                guna2TextBox2.UseSystemPasswordChar = true;
            }
        }

        private void label2_MouseHover(object sender, EventArgs e)
        {
            label2.Font = new Font(label2.Font.Name, label2.Font.SizeInPoints, FontStyle.Underline);
        }

        private void label2_MouseLeave(object sender, EventArgs e)
        {
            label2.Font = new Font(label2.Font.Name, label2.Font.SizeInPoints, FontStyle.Regular);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (guna2TextBox1.Text != "" && guna2TextBox2.Text != "")
            {
                try
                {
                    Connect();
                    data dt = new data();
                    string tk = guna2TextBox1.Text;
                    string mk = Encrypt(guna2TextBox2.Text);
                    dt.tk = tk;
                    dt.mk = mk;
                    dt.style = 1;
                    Send(dt);
                    guna2Button1.Enabled = false;
                    //lưu tài khoản ,mật khẩu
                    string path = @"tk_mk.txt";
                    if (guna2CheckBox1.Checked == true)
                    {
                        String content = guna2TextBox1.Text + "\n" + guna2TextBox2.Text + "\n" + "1";
                        File.WriteAllText(path, content);
                    }
                    else
                    {
                        if (File.Exists(path))
                            File.Delete(path);
                    }
                }
                catch { }
            }
            else
                MessageBox.Show("Vui lòng nhập tài khoản,mật khẩu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
       
        private void guna2TextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (guna2TextBox1.Text != "" && guna2TextBox2.Text != "")
                {
                    try
                    {
                        Connect();
                        data dt = new data();
                        string tk = guna2TextBox1.Text;
                        string mk = Encrypt(guna2TextBox2.Text);
                        dt.tk = tk;
                        dt.mk = mk;
                        dt.style = 1;
                        Send(dt);
                        guna2Button1.Enabled = false;
                        //lưu tài khoản ,mật khẩu
                        string path = @"tk_mk.txt";
                        if (guna2CheckBox1.Checked == true)
                        {
                            String content = guna2TextBox1.Text + "\n" + guna2TextBox2.Text + "\n" + "1";
                            File.WriteAllText(path, content);                       
                        }
                        else
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                        }
                    }
                    catch { }
                }
                else
                    MessageBox.Show("Vui lòng nhập tài khoản,mật khẩu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void guna2CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
           
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (guna2TextBox1.Text != "" && guna2TextBox2.Text != "")
                {
                    try
                    {
                        Connect();
                        data dt = new data();
                        string tk = guna2TextBox1.Text;
                        string mk = Encrypt(guna2TextBox2.Text);
                        dt.tk = tk;
                        dt.mk = mk;
                        dt.style = 1;
                        Send(dt);
                        guna2Button1.Enabled = false;
                        //lưu tài khoản ,mật khẩu
                        string path = @"tk_mk.txt";
                        if (guna2CheckBox1.Checked == true)
                        {
                            String content = guna2TextBox1.Text + "\n" + guna2TextBox2.Text + "\n" + "1";
                            File.WriteAllText(path, content);
                        }
                        else
                        {
                            if (File.Exists(path))
                                File.Delete(path);
                        }
                    }
                    catch { }
                }
                else
                    MessageBox.Show("Vui lòng nhập tài khoản,mật khẩu", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Login login = (Login)(this.ParentForm);
            login.Forgot_pass();
        }
    }
}
