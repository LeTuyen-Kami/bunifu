using Bunifu.UI.WinForms;
using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Security.Cryptography;

namespace bunifu
{
    public partial class none : Form
    {
        chatbox chat;
        IPEndPoint IP;
        Socket client;
        bool isconnect;
        int dem = 0;
        DateTime now = DateTime.Now;
        string Id_M = "1";
        public none()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//tránh việc đụng độ khi sử dụng tài nguyên giữa các thread
            but_old.Location=guna2TextBox1.Location;
        }
        public void Id(string id)
        {
            Id_M = id;
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
        Guna2Button but_old = new Guna2Button();
        void Receive()
        {
            try
            {
                while (true)
                {
                    //khai báo mảng byte để nhận dữ liệu dưới mảng byte
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    //chuyển data từ dạng byte sang dạng string
                    data message = (data)Deseriliaze(data);
                    if (message.style==1)
                    {
                        if (message.id_recv.ToString() == Id_M)
                            message.id_recv = message.id_send;
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            Guna2Button button = new Guna2Button();
                            button.Location = but_old.Location;
                            button.Size = guna2Button3.Size;
                            //button.Dock = DockStyle.Top;
                            button.Top = but_old.Bottom;
                            button.Text = message.id_recv.ToString();
                            button.Click += new EventHandler(guna2Button3_Click);
                            panel3.Controls.Add(button);
                            but_old = button;
                        });
                    }    
                    if (message.style==10)
                    {
                        DataSet ds = message.ds;
                        DataTable dt = ds.Tables[0];
                        foreach (DataRow row in dt.Rows)
                        {
                            foreach (DataColumn col in dt.Columns)
                            {
                                if (row[col].ToString()!=Id_M)
                                {
                                    if (dem == 0)
                                    {
                                        this.BeginInvoke((MethodInvoker)delegate ()
                                        {
                                            showchatbox(row[col].ToString());
                                        });
                                        dem++;
                                    }
                                    this.BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        Guna2Button button = new Guna2Button();
                                        button.Location = but_old.Location;
                                        button.Size = guna2Button3.Size;
                                        //button.Dock = DockStyle.Top;
                                        button.Top = but_old.Bottom;
                                        button.Text = row[col].ToString();
                                        button.Click += new EventHandler(guna2Button3_Click);
                                        panel3.Controls.Add(button);
                                        but_old = button;
                                    });
                                }    
                            }    
                        }
                    }   
                    else
                    {
                        string s = now.Day.ToString() + "/" + now.Month + " " + now.Hour.ToString() + ":" + now.Minute.ToString() + ":" + now.Second.ToString();
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            if (message.id_send.ToString() != Id_M)
                            {
                                message.msg = Decrypt(message.msg);
                                chat.addinmessage(message.msg,s, buble.msgtype.In);
                            }
                        });

                    }
                }
            }
            catch
            {
                Close();
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
        private void chatbox1_Load_1(object sender, EventArgs e)
        {

        }
        private void none_Load(object sender, EventArgs e)
        {        
            Connect();
            data dt = new data();
            dt.style = 3;
            int id = int.Parse(Id_M);
            dt.id_send = id;
            Send(dt);
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuImageButton2_MouseHover(object sender, EventArgs e)
        {
            panel2.Size = panel2.MaximumSize;
        }

        private void bunifuImageButton2_MouseLeave(object sender, EventArgs e)
        {
        }

        private void panel2_MouseLeave(object sender, EventArgs e)
        {
            panel2.Size = panel2.MinimumSize;

        }
        
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string temp;
            temp = guna2TextBox1.Text;
            if (temp=="")
            {
                guna2DataGridView1.Visible = false;
            }
            
            else
            {
                if (temp == "all")
                    temp = "";
                DataSet data = new DataSet();
                string query = "select ten from Account where Id LIKE '" + temp + "' + '%'";
                using (SqlConnection sql = new SqlConnection
                    ("Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True"))
                {
                    sql.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, sql);
                    adapter.Fill(data);
                    guna2DataGridView1.Visible = true;
                    guna2DataGridView1.AutoResizeRows();
                    DataTable dt = data.Tables[0];
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (DataColumn col in dt.Columns)
                        {
                            row[col] = Decrypt(row[col].ToString());
                        }
                    }
                    guna2DataGridView1.DataSource = dt;
                    guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                        guna2DataGridView1.RowCount * guna2DataGridView1.RowTemplate.Height);
                    sql.Close();
                }
            }               
        }
        public void showchatbox(string s)
        {
            chat = new chatbox();
            chat.Location = panel1.Location;
            chat.Dock = DockStyle.Fill;
            chat.Size = panel1.Size;
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
                    string message =Decrypt(dta.GetString(3));
                    string time = dta.GetString(4);
                    int id = dta.GetInt32(1);
                    if (id.ToString() == Id_M)
                    {
                        chat.addinmessage(message, time, buble.msgtype.Out);
                    }
                    else
                        chat.addinmessage(message, time, buble.msgtype.In);
                }
            }
            strConnect.Close();
            panel1.Controls.Add(chat);
            chat.vertical();
            chat.BringToFront();
        }
        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Guna2Button a = sender as Guna2Button;
            string text = a.Text;
            showchatbox(text);
        }
        SqlConnection strConnect = new SqlConnection();
        public void Create_Connect()
        {
            string str = "Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True";
            strConnect.ConnectionString = str;
            strConnect.Open();
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                data dt = new data();
                dt.style = 4;
                dt.id_send =int.Parse(Id_M);
                dt.id_recv =int.Parse(guna2TextBox1.Text);
                Send(dt);
                guna2TextBox1.Text = "";
                MessageBox.Show("Đã kêt bạn thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void none_FormClosing(object sender, FormClosingEventArgs e)
        {
            close();
            Application.Exit();
        }
    }
}
