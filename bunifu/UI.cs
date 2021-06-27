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
        bool EnableA = true;
        Danhsach_tinnhan danhsach = new Danhsach_tinnhan();
        byte[] my_img;
        string My_name = "";
        public none()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//tránh việc đụng độ khi sử dụng tài nguyên giữa các thread
            random = new Random();
            listbutton = new List<Guna2Button>() {guna2Button1,guna2Button2,guna2Button3,guna2Button4,guna2Button5,guna2Button6,guna2Button7 };
            this.ControlBox = false;
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
                    byte[] data = new byte[1024 * 10000];
                    client.Receive(data);
                    //chuyển data từ dạng byte sang dạng string
                    data message = (data)Deseriliaze(data);
                    if (message.style == 1)
                    {
                        if (message.id_recv.ToString() == Id_M)
                        {
                            DataSet ds = message.ds;
                            guna2Button2.Image = new Bitmap(@"notification.png");
                            dataTable2 = ds.Tables["ketban"];
                        }                     
                    }
                    if (message.style == 2)
                    {
                        DataSet ds = message.ds;

                        if (message.id_send.ToString() == Id_M)
                        {
                            dataTable2 = ds.Tables["ketban1"];
                            datafriend = ds.Tables["ban2"];
                        }
                        if (message.id_recv.ToString() == Id_M)
                        {
                            guna2Button2.Image = new Bitmap(@"notification.png");
                            dataTable2 = ds.Tables["ketban2"];
                            datafriend = ds.Tables["ban1"];

                        }
                    }
                    if (message.style == 3)
                    {
                        DataSet ds = message.ds;
                        if (message.id_recv.ToString() == Id_M)
                        {
                            datafriend = ds.Tables["ban2"];
                        }
                        if (message.id_send.ToString() == Id_M)
                        {
                            datafriend = ds.Tables["ban1"];
                        }
                    }
                    if (message.style == 4)
                    {
                        foreach (DataRow row in message.ds.Tables["Thanhvien"].Rows)
                        {
                            if (row["Id"].ToString() == Id_M&&message.msg=="success")
                            {
                                if (message.tk!=My_name)
                                {
                                    guna2Button2.Image = new Bitmap(@"notification.png");
                                    dataTable2.Rows.Add(new object[] { message.ngaysinh, 99999999, Id_M, message.tk, 0 });
                                }                          
                                data_nhom.Rows.Add(new object[] { message.id, message.ten });
                                break;
                            }
                        }
                    }
                    if (message.style==5)
                    {
                        foreach (DataRow row in datafriend.Rows)
                        {
                            if (row["Id"].ToString()==message.id)
                            {
                                row["Trangthai"] = message.msg;
                            }    
                        }    
                    }    
                    if (message.style == 10)
                    {
                        DataSet ds = message.ds;
                        dataTable1 = ds.Tables["data"];
                        dataTable2 = ds.Tables["ketban"];
                        data_nhom = ds.Tables["nhom"];
                        string[] source = dataTable1.Rows[0]["Ten"].ToString().Split(new char[] { ' ' });
                        label1.Text = source[source.Count() - 1];
                        My_name = label1.Text;
                        MemoryStream mem = new MemoryStream((byte[])dataTable1.Rows[0][4]);
                        guna2CirclePictureBox1.Image = System.Drawing.Image.FromStream(mem);
                        guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                        datafriend = ds.Tables["ban"];
                        if (checkread(dataTable2))
                        {
                            guna2Button2.Image = new Bitmap(@"notification.png");
                        }
                    }
                    if (message.style == 0)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            if (message.loai_mes == "0")
                            {
                                if (message.id_recv.ToString() == Id_M)
                                {
                                    danhsach.add_mes(message, 0);
                                }
                            }
                            if (message.loai_mes == "1")
                            {
                                if (check_have(message.id_recv.ToString())!="")
                                {
                                    danhsach.add_mes(message, 1);
                                }
                            }
                            if (message.ds.Tables["Tag"]!=null&& check_have(message.id_recv.ToString())!=""&& message.loai_mes == "1"&& message.loai_mes == "0")
                                foreach (DataRow row in message.ds.Tables["Tag"].Rows)
                                {
                                    if (row["Id"].ToString() == Id_M)
                                    {
                                        guna2Button2.Image = new Bitmap(@"notification.png");
                                        dataTable2.Rows.Add(new object[] { message.tk,99999999, Id_M, message.ten, 0 });
                                    }
                                }    
                        });
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
            string temp = "";

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
                dt.id_send = int.Parse(Id_M);
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
            if (activeForm != null)
                panel4.Controls.Remove(activeForm);
            Color color = SelectThemeColor();
            panel1.BackColor = color;
            Myinfo minfo = new Myinfo(color);
            activeForm = minfo;
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
            if (activeForm != null)
                panel4.Controls.Remove(activeForm);
            Color color = ActivateButton(sender);
            panel1.BackColor = color;
            Setnotice noctice = new Setnotice(color);
            activeForm = noctice;
            noctice.set(dataTable2, Id_M);
            panel4.Controls.Add(noctice);
            foreach (DataRow row in dataTable2.Rows)
            {
                row["Readed"] = "1";
            }
            guna2Button2.Image = new Bitmap(@"notification1.png");
            noctice.Dock = DockStyle.Fill;
            noctice.BringToFront();
            data dt = new data();
            dt.style = 6;
            dt.id_send = int.Parse(Id_M);
            Send(dt);
        }
        public void changeImage(System.Drawing.Image image)
        {
            guna2CirclePictureBox1.Image = image;
        }      
        private void guna2Button4_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
                panel4.Controls.Remove(activeForm);
            Color color = ActivateButton(sender);
            panel1.BackColor = color;
            panel1.BackColor = color;
            Friend friend = new Friend(color);
            activeForm = friend;
            friend.Setfriend(datafriend, Id_M);
            panel4.Controls.Add(friend);
            friend.Dock = DockStyle.Fill;
            friend.BringToFront();

        }
        public void addcontrols(string id, int a,Color color)
        {
            Info info = new Info(color);
            info.AddId(id, a, Id_M, EnableA);
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
                if (row["Nguoigui"].ToString() == id1)
                {
                    row["Status"] = "Refuse";
                }
            }
        }
        public string check_have(string s)
        {
            foreach (DataRow row in data_nhom.Rows)
            {
                if (s == row["Id_nhom"].ToString())
                    return row["Tennhom"].ToString();
            }
            return "";
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
                panel4.Controls.Remove(activeForm);
            Color color= ActivateButton(sender);
            panel1.BackColor = color;
            my_img = (byte[])dataTable1.Rows[0]["Img"];
            danhsach = new Danhsach_tinnhan(color);
            activeForm = danhsach;
            panel4.Controls.Add(danhsach);
            danhsach.nhap_data(datafriend, data_nhom, Id_M, my_img, My_name);
            danhsach.Dock = DockStyle.Fill;
            danhsach.BringToFront();
        }
        private void guna2Button3_Click_1(object sender, EventArgs e)
        {
            Color color = ActivateButton(sender);
            panel1.BackColor = color;
            Taonhom taonhom = new Taonhom(color);
            DataTable tam = new DataTable("Friend");
            tam = datafriend.Copy();
            taonhom.nhap(tam, Id_M,My_name);
            taonhom.ShowDialog();
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
            Color color = ActivateButton(sender);
            panel1.BackColor = color;
            if (MessageBox.Show("Are you sure you want to exit?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                data dt = new data();
                dt.style = 14;
                dt.id = Id_M;
                Send(dt);
                Application.Exit();
            }
        }

        private void guna2Button5_Click(object sender, EventArgs e)
        {
            Color color= ActivateButton(sender);
            panel1.BackColor = color;
            if (MessageBox.Show("Do you want to sign out?", "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                data dt = new data();
                dt.style = 14;
                dt.id = Id_M;
                Send(dt);
                Login login = new Login();
                login.Show();
                this.Hide();
            }
        }
        //chỉnh màu
        private Guna2Button currentButton;
        private Random random;
        private int tempIndex;
        private UserControl activeForm;
        private Color SelectThemeColor()
        {
            int index = random.Next(ThemeColor.ColorList.Count);
            while (tempIndex == index)
            {
                index = random.Next(ThemeColor.ColorList.Count);
            }
            tempIndex = index;
            string color = ThemeColor.ColorList[index];
            return ColorTranslator.FromHtml(color);
        }
        private Color ActivateButton(object btnSender)
        {
            Color color = Color.FromArgb(253, 112, 161);
            if (btnSender != null)
            {
                if (currentButton != (Guna2Button)btnSender)
                {
                    DisableButton();
                    color = SelectThemeColor();
                    currentButton = (Guna2Button)btnSender;
                    currentButton.FillColor = color;

                    currentButton.ForeColor = Color.White;
                    ThemeColor.PrimaryColor = color;
                    ThemeColor.SecondaryColor = ThemeColor.ChangeColorBrightness(color, -0.3);
                }
            }
            return color;
        }
        List<Guna2Button> listbutton;
        private void DisableButton()
        {
            
            foreach (Guna2Button previousBtn in listbutton)
            {
                if (previousBtn.GetType() == typeof(Guna2Button))
                {
                    previousBtn.FillColor = Color.FromArgb(253, 112, 161);
                    previousBtn.ForeColor = Color.Gainsboro;
                    //previousBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                }
            }
        }
        Guna2Button Min;
        private void panelMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Min != null)
                    panelMenu.Controls.Remove(Min);
                Min = new Guna2Button();
                Min.Location = e.Location;
                Min.Size = new Size(100, 25);
                Min.FillColor = Color.Transparent;
                Min.Text = "Minimize";
                Min.BringToFront();
                panelMenu.Controls.Add(Min);
                Min.Click += new EventHandler(minimize_click);
            }
            else
            {
                if (Min != null)
                    panelMenu.Controls.Remove(Min);
            }
        }
        private void minimize_click(object sender,EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            if (Min!=null)
                panelMenu.Controls.Remove(Min);
        }

        private void panelMenu_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button7_Click(object sender, EventArgs e)
        {
            if (activeForm != null)
                panel4.Controls.Remove(activeForm);
            Color color = ActivateButton(sender);
            panel1.BackColor = color;
            About about = new About(color);
            activeForm = about;
            panel4.Controls.Add(about);
            about.Dock = DockStyle.Fill;
            about.BringToFront();
        }
    }
}
