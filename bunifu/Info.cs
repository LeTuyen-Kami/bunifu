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

namespace bunifu
{
    public partial class Info : UserControl
    {
        IPEndPoint IP;
        Socket client;
        bool isconnect;
        int phanloai;
        string id_ban;
        string my_id;
        public Info()
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
                    if (dt.style==100)
                    {
                        if (phanloai == 1)
                            guna2Button2.Visible = false;
                        if (phanloai == 0)
                            guna2Button1.Visible = false;
                        DataTable datainfo = new DataTable();
                        datainfo = dt.ds.Tables["Info"];
                        MemoryStream mem = new MemoryStream((byte[])datainfo.Rows[0]["Img"]);
                        guna2PictureBox1.Image =System.Drawing.Image.FromStream(mem);
                        label6.Text = datainfo.Rows[0]["Ten"].ToString();
                        label7.Text = datainfo.Rows[0]["Sex"].ToString();
                        label8.Text= datainfo.Rows[0]["Ngaysinh"].ToString();
                        label9.Text= datainfo.Rows[0]["Id"].ToString();
                    }    
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
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            data dt = new data();
            dt.style = 9;
            dt.id_send =int.Parse(my_id);
            dt.id_recv = int.Parse(id_ban);
            Send(dt);
            guna2Button1.Enabled = false;
            none no = (none)(this.ParentForm);
            no.setenanle(false);
            label5.Visible = true;
            label5.Text = "Sent friend request";
        }
        public void AddId(string id,int a,string myid,bool enable)
        {
            my_id = myid;
            id_ban = id;
            phanloai = a;
            Connect();
            data dt = new data();
            dt.id_send =int.Parse(id);
            dt.style = 8;
            Send(dt);
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            guna2Button2.Enabled = false;
            data dt = new data();
            dt.style = 12;
            dt.id_send =int.Parse(my_id);
            dt.id_recv =int.Parse(id_ban);
            Send(dt);
            //none no = (none)(this.ParentForm);
            //no.unfriend(id_ban);
        }
    }
}
