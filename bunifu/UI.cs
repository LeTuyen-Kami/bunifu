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
        IPEndPoint IP;
        Socket client;
        bool isconnect;
        DataTable data_nhom = new DataTable();
        int dem = 0;
        DateTime now = DateTime.Now;
        DataTable dataTable1 = new DataTable();
        DataTable dataTable2 = new DataTable();
        DataTable datafriend = new DataTable();
        string Id_M = "1";
        bool EnableA=true;
        Danhsach_tinnhan danhsach = new Danhsach_tinnhan();
        public none()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//tránh việc đụng độ khi sử dụng tài nguyên giữa các thread
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
                    if (message.style == 1)
                    {
                        if (message.id_recv.ToString()==Id_M)
                        {
                            DataSet ds = message.ds;
                            guna2Button2.Image = new Bitmap(@"D:\Lập trình mạng\bunifu\notification.png");
                            dataTable2 = ds.Tables["ketban"];
                        }

                        //if (message.id_recv.ToString() == Id_M)
                        //    message.id_recv = message.id_send;
                        //this.BeginInvoke((MethodInvoker)delegate ()
                        //{
                        //    Guna2Button button = new Guna2Button();
                        //    button.Location = but_old.Location;
                        //    button.Size = guna2Button3.Size;
                        //    //button.Dock = DockStyle.Top;
                        //    button.Top = but_old.Bottom;
                        //    button.Text = message.id_recv.ToString();
                        //    button.Click += new EventHandler(guna2Button3_Click);
                        //    panel3.Controls.Add(button);
                        //    but_old = button;
                        //});
                    }  
                    if (message.style==2)
                    {
                        DataSet ds = message.ds;
                        
                        if (message.id_send.ToString()==Id_M)
                        {
                            dataTable2 = ds.Tables["ketban1"];
                            datafriend = ds.Tables["ban2"];
                        }  
                        if (message.id_recv.ToString()==Id_M)
                        {
                            guna2Button2.Image = new Bitmap(@"D:\Lập trình mạng\bunifu\notification.png");
                            dataTable2 = ds.Tables["ketban2"];
                            datafriend = ds.Tables["ban1"];

                        }    
                    } 
                    if (message.style==3)
                    {
                        DataSet ds = message.ds;
                        if (message.id_recv.ToString()==Id_M)
                        {
                            datafriend = ds.Tables["ban2"];
                        }  
                        if (message.id_send.ToString()==Id_M)
                        {
                            datafriend = ds.Tables["ban1"];
                        }    
                    }   
                    if (message.style==4)
                    {
                        foreach (DataRow row in message.ds.Tables["Thanhvien"].Rows)
                        {
                            if (row["Id"].ToString()==Id_M)
                            {
                                data_nhom.Rows.Add(new object[] { message.id,message.ten });
                                break;
                            }    
                        }    
                    }    
                    if (message.style == 10)
                    {
                        DataSet ds = message.ds;
                        dataTable1 = ds.Tables["data"];
                        dataTable2 = ds.Tables["ketban"];
                        data_nhom = ds.Tables["nhom"];
                        MemoryStream mem = new MemoryStream((byte[])dataTable1.Rows[0][4]);
                        guna2CirclePictureBox1.Image = Image.FromStream(mem);
                        guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        datafriend = ds.Tables["ban"];
                        if (checkread(dataTable2))
                        {
                            guna2Button2.Image = new Bitmap(@"D:\Lập trình mạng\bunifu\notification.png");
                        }
                    }
                    if (message.style == 0)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            if (message.id_recv.ToString() == Id_M)
                            {
                                danhsach.add_mes(message.msg);
                            }
                        });
                    }
                }
            }
            catch(Exception e)
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
        private void chatbox1_Load_1(object sender, EventArgs e)
        {

        }
        private void none_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1200, 740);
            Connect();
            data dt = new data();
            dt.style = 3;
            int id = int.Parse(Id_M);
            dt.id_send = id;
            Send(dt);
        }
        public void recvdata(DataTable dataTable)
        {
            dataTable1 = dataTable;
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string temp="";
            
                DataSet data = new DataSet();
                string query = "select Ten from Account where Id LIKE '" + temp + "' + '%'";
            using (SqlConnection sql = new SqlConnection
                ("Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True"))
            {
                sql.Open();
                SqlDataAdapter adapter = new SqlDataAdapter(query, sql);
                adapter.Fill(data);
                DataTable dt = data.Tables[0];
                sql.Close();
            }
        }
        SqlConnection strConnect = new SqlConnection();
        public void Create_Connect()
        {
            string str = "Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True";
            strConnect.ConnectionString = str;
            strConnect.Open();
        }
        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                data dt = new data();
                dt.style = 4;
                dt.id_send =int.Parse(Id_M);
                Send(dt);
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

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {
            Myinfo minfo = new Myinfo();
            minfo.RId(Id_M);
            minfo.Dock = DockStyle.Fill;
            panel4.Controls.Add(minfo);
            minfo.BringToFront();
            minfo.datatable(dataTable1);
            
        }
        public bool checkread(DataTable dataTable)
        {
            foreach (DataRow row in dataTable.Rows)
                if ((int)row["Readed"] == 0)
                    return true;
            return false;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {

            Setnotice noctice = new Setnotice();
            noctice.set(dataTable2,Id_M);
            panel4.Controls.Add(noctice);
            foreach (DataRow row in dataTable2.Rows)
            {
                row["Readed"] = "1";
            }
            guna2Button2.Image = new Bitmap(@"D:\Lập trình mạng\bunifu\notification1.png");
            noctice.Dock = DockStyle.Fill;
            noctice.BringToFront();
            data dt = new data();
            dt.style = 6;
            dt.id_send =int.Parse(Id_M);
            Send(dt);
        }
        public void changeImage(Image image)
        {
            guna2CirclePictureBox1.Image = image;
        }
        //public void unfriend(string id)
        //{
        //    int i = 0;
        //    foreach(DataRow row in datafriend.Rows)
        //    {
        //        if (row["Id"].ToString() == id)
        //        {
        //            datafriend.Rows.RemoveAt(i);
        //            return;
        //        }
        //        i++;
        //    }    
        //}
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Friend friend = new Friend();
            friend.Setfriend(datafriend,Id_M);
            panel4.Controls.Add(friend);
            friend.Dock = DockStyle.Fill;
            friend.BringToFront();

        }
        public void addcontrols(string id,int a)
        {
            Info info = new Info();
            info.AddId(id,a,Id_M,EnableA);
            panel4.Controls.Add(info);
            info.Dock = DockStyle.Fill;
            info.BringToFront();
        }
        public void setenanle(bool enable)
        {
            EnableA = enable;
        }
        public void UpdateKetban(string id1)
        {
            foreach (DataRow row in dataTable2.Rows)
            {
                if (row["Nguoigui"].ToString()==id1)
                {
                    row["Status"] = "Refuse";
                }    
            }    
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            danhsach = new Danhsach_tinnhan();
            panel4.Controls.Add(danhsach);
            danhsach.nhap_data(datafriend,data_nhom, Id_M);
            danhsach.Dock = DockStyle.Fill;
            danhsach.BringToFront();
        }
        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            Taonhom taonhom = new Taonhom();
            taonhom.nhap(datafriend,Id_M);
            taonhom.ShowDialog();
        }
    }
}
