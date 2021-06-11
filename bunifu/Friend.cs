using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace bunifu
{
    public partial class Friend : UserControl
    {
        IPEndPoint IP;
        Socket client;
        bool isconnect;
        public Friend()
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
                    data dt = (data)Deseriliaze(datat);
                    if (dt.style==111)
                    {
                        guna2DataGridView1.Visible = true;
                        guna2DataGridView1.AutoResizeRows();
                        guna2DataGridView1.DataSource = dt.ds.Tables["banbe"];
                        guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                            (guna2DataGridView1.RowCount - 1) * guna2DataGridView1.RowTemplate.Height);
                    }    
                }
            }
            catch
            {
                close();
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
        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
            string temp;
            temp = guna2TextBox1.Text;
            if (temp == "")
            {
                guna2DataGridView1.Visible = false;
            }
        }
        public void Setfriend(DataTable dataTable)
        {           
            foreach (DataRow row in dataTable.Rows)
            {
                string s = row["Ten"].ToString();
                string id = row["Id"].ToString();
                if (s!="")
                {
                    Banbe banbe = new Banbe();
                    banbe.Addten(s,id);
                    banbe.Dock = DockStyle.Top;
                    panel3.Controls.Add(banbe);
                }    
                
            }            
        }

        private void guna2TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Connect();
                string temp;
                temp = guna2TextBox1.Text;
                if (temp == "all")
                    temp = "";
                data dt = new data();
                dt.style = 7;
                dt.msg = temp;
                Send(dt);
            }   
        }
    }
}
