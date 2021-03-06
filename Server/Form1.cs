using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using bunifu;

namespace Server
{
    public partial class Form1 : Form
    {
        SqlConnection strConnect = new SqlConnection();
        DateTime now = new DateTime();
        bool isconnect;
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;//tránh việc đụng độ khi sử dụng tài nguyên giữa các thread
            this.SetStyle(ControlStyles.ResizeRedraw, true);
        }
        IPEndPoint IP;
        Socket server;
        //khai báo 1 list các client
        List<Socket> clientList;

        //kết nối đến server
        void Connect()
        {
            clientList = new List<Socket>();//khởi tạo 1 list nhiều client
            //khởi tạo địa chỉ IP và socket để kết nối
            IP = new IPEndPoint(IPAddress.Any, 1997);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            //đợi kết nối từ client
            server.Bind(IP);
            //tạo 1 luồng lăng nghe từ client
            Thread Listen = new Thread(() =>
            {
                try
                {
                    while (true)
                    {

                        server.Listen(100);
                        Socket client = server.Accept();//nếu lăng nghe thành công thì server chấp nhận kết nối
                        clientList.Add(client);//thêm các client được server accept vào list
                        //tạo luồng nhận thông tin từ client
                        isconnect = server.Connected;
                        AddMessage("Đã có client kết nối");
                        Thread receive = new Thread(Receive);
                        
                        receive.IsBackground = true;
                        receive.Start(client);
                    }
                }
                /*khi kết nối đến n client mà có 1 client disconnect thì server sẽ chạy vòng lặp while liên tục để
                 chương trình ko bị crash*/
                catch
                {
                    IP = new IPEndPoint(IPAddress.Any, 1997);
                    server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                }
            });
            Listen.IsBackground = true;
            Listen.Start();
        }

        //đóng kết nối đến server
        void Close()
        {
            if (isconnect)
            {
                server.Close();
            }
        }

        //gửi dữ liệu
        void Send(Socket client)
        {
            //nếu textboc khác rỗng thì mới gửi tin
            if ((client != null) && (txtMessage.Text != string.Empty))
            {
                client.Send(Serialize(txtMessage.Text));
            }
        }

        //nhận dữ liệu
        public void Create_Connect()
        {
            string str = "Data Source=LAPTOP-EGRB430C\\SQLEXPRESS;Initial Catalog=ten;Integrated Security=True";
            strConnect.ConnectionString = str;
            strConnect.Open();
        }
        
        void Receive(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    //khởi tạo mảng byte để nhận dữ liệu
                    byte[] datat = new byte[1024 * 10000];
                    client.Receive(datat);

                    //chuyển data từ dạng byte sang dạng string
                    data message = (data)Deseriliaze(datat);
                    AddMessage(message.style.ToString());
                    //Thêm tin nhắn vào csdl
                    if (message.style == 0)
                    {
                        data dt = new data();
                        dt.style = 0;
                        dt.ten = message.ten;
                        dt.msg = message.msg;
                        dt.id_recv = message.id_recv;
                        dt.img = message.img;
                        dt.image = message.image;
                        dt.loai_mes = message.loai_mes;
                        dt.loai_nhan = message.loai_nhan;
                        dt.id_send = message.id_send;
                        dt.tk = "N', đã nhắc đến bạn trong nhóm " + message.tk + "'";
                        dt.ds = message.ds;
                        Create_Connect();
                        string sql = "Insert into Message (Id_S, Id_R, Message,Time,Loai_mes,Loai_nhan,Data_byte) "
                                                         + " values (@s, @r, @msg,@time,@lm,@ln,@dtb) ";
                        SqlCommand cmd = strConnect.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@s", SqlDbType.Int).Value = message.id_send;
                        cmd.Parameters.Add("@r", SqlDbType.Int).Value = message.id_recv;
                        cmd.Parameters.Add("@msg", SqlDbType.NVarChar).Value = message.msg;
                        cmd.Parameters.Add("@time", SqlDbType.NVarChar).Value = message.time;
                        cmd.Parameters.Add("@lm", SqlDbType.NVarChar).Value = message.loai_mes;
                        cmd.Parameters.Add("@ln", SqlDbType.NVarChar).Value = message.loai_nhan;
                        cmd.Parameters.Add("@dtb", SqlDbType.Image).Value = message.image;
                        cmd.ExecuteNonQuery();
                        if (message.ds.Tables["Tag"]!=null)
                        {
                            foreach (DataRow row in message.ds.Tables["Tag"].Rows)
                            {
                                sql = "Insert into Ketban (Nguoigui,Nguoinhan,Status,Readed) values (" + dt.id_send + "," + row["Id"] + "," + dt.tk + "," + 0 + ")";
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        strConnect.Close();
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null && item != client)
                                    {
                                        item.Send(Serialize(dt));
                                    }
                                }
                            });
                        }
                        catch { }

                    }
                    //Đăng nhập
                    if (message.style == 1)
                    {
                        data dt = new data();
                        int Id = 0;
                        Create_Connect();
                        string sqll = "select Id from Account where Taikhoan='" + message.tk + "' and Matkhau='" + message.mk + "'";
                        SqlCommand cmd = new SqlCommand(sqll, strConnect);
                        SqlDataReader dta = cmd.ExecuteReader();
                        if (dta.Read() == true)
                        {
                            int IdIndex = dta.GetOrdinal("Id");
                            Id = dta.GetInt32(IdIndex);
                            client.Send(Serialize(Id.ToString()));
                        }
                        else
                        {
                            int temp = 0;
                            client.Send(Serialize(temp.ToString()));
                        }
                        dta.Close();
                        if (Id != 0)
                        {
                            sqll = "update Account set Trangthai='on' where Id='" + Id + "'";
                            cmd.CommandText = sqll;
                            cmd.ExecuteNonQuery();
                        }
                        strConnect.Close();
                        if (Id != 0)
                        {
                            dt.id = Id.ToString();
                            dt.msg = "on";
                            dt.style = 5;
                            try
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    foreach (Socket item in clientList.ToList())
                                    {
                                        if (item != null && item != client)
                                        {
                                            item.Send(Serialize(dt));
                                        }
                                    }
                                });
                            }
                            catch { }
                        }
                    }
                    //Tạo tài khoản
                    if (message.style == 2)
                    {
                        if (check_name(message.tk, message.ten, "", 0))
                        {
                            int temp = 1;
                            client.Send(Serialize(temp.ToString()));
                        }
                        else
                        {
                            Create_Connect();
                            string sql = "Insert into Account (Taikhoan, Matkhau, Ten,Ngaytao,Img,Trangthai) "
                                                             + " values (@tk, @mk, @ten,@nt,@img,@tt) ";
                            SqlCommand cmd = strConnect.CreateCommand();
                            cmd.CommandText = sql;
                            SqlParameter gradeParam = new SqlParameter("@tk", SqlDbType.NVarChar);
                            gradeParam.Value = message.tk;
                            cmd.Parameters.Add(gradeParam);
                            SqlParameter highSalaryParam = cmd.Parameters.Add("@mk", SqlDbType.NVarChar);
                            highSalaryParam.Value = message.mk;
                            cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = message.ten;
                            string s = now.Day.ToString() + "/" + now.Month + "/" + now.Year;
                            cmd.Parameters.Add("@nt", SqlDbType.NVarChar).Value = s;
                            cmd.Parameters.Add("@img", SqlDbType.Image).Value = message.img;
                            cmd.Parameters.Add("@tt", SqlDbType.NVarChar).Value = "on";
                            cmd.ExecuteNonQuery();
                            int temp = 0;
                            client.Send(Serialize(temp.ToString()));
                            strConnect.Close();
                            //strConnect.Dispose();
                        }
                    }
                    //Lấy danh sách bạn,danh sách kết bạn và thông tin tài khoản đăng nhập
                    if (message.style == 3)
                    {
                        data dt = new data();
                        DataSet ds = new DataSet();
                        dt.style = 10;
                        Create_Connect();
                        string sqll = "select Account.Ten,Account.Id,Account.Img,Account.Trangthai from Account,Friend where Friend.Id_M='" + message.id_send + "' and Friend.Id_N=Account.Id or Friend.Id_N='" + message.id_send + "' and Friend.Id_M=Account.Id";
                        SqlDataAdapter adapter = new SqlDataAdapter(sqll, strConnect);
                        adapter.Fill(ds, "ban");
                        sqll = "select * from Account where Id='" + message.id_send + "'";
                        adapter.SelectCommand.CommandText = sqll;
                        adapter.Fill(ds, "data");
                        sqll = "select Ketban.Status,Ketban.Nguoigui,Ketban.Nguoinhan,Account.Ten,Ketban.Readed from Account,Ketban where Ketban.Nguoinhan='" + message.id_send + "'and Ketban.Nguoigui=Account.Id or Ketban.Nguoigui='" + message.id_send + "' and Ketban.Nguoinhan=Account.Id and Ketban.Status='Accept'";
                        adapter.SelectCommand.CommandText = sqll;
                        adapter.Fill(ds, "ketban");
                        sqll = "select Thanhvien.Id_nhom,Nhom.Tennhom from Thanhvien,Nhom where Thanhvien.Id_nhom=Nhom.Id and Thanhvien.Id_account='" + message.id_send + "'";
                        adapter.SelectCommand.CommandText = sqll;
                        adapter.Fill(ds, "nhom");
                        dt.ds = ds;
                        client.Send(Serialize(dt));
                        strConnect.Close();
                    }
                    //Thêm bạn vào danh sách bạn
                    if (message.style == 4)
                    {
                        Create_Connect();
                        string sql = "Insert into Friend (Id_M, Id_N) "
                                                         + " values (@m, @n) ";
                        SqlCommand cmd = strConnect.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@m", SqlDbType.Int).Value = message.id_send;
                        cmd.Parameters.Add("@n", SqlDbType.Int).Value = message.id_recv;
                        cmd.ExecuteNonQuery();
                        strConnect.Close();
                        data dt = new data();
                        dt.style = 1;
                        dt.id_send = message.id_send;
                        dt.id_recv = message.id_recv;
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null)
                                    {
                                        item.Send(Serialize(dt));
                                    }
                                }
                            });
                        }
                        catch { }
                    }
                    //update tài khoản,thay đỗi thông tin
                    if (message.style == 5)
                    {
                        data dt = new data();
                        if (check_name(message.tk, message.ten, message.id, 3))
                        {
                            dt.style = 0;
                            client.Send(Serialize(dt));
                        }
                        if (check_name(message.email, "", message.id, 2))
                        {
                            dt.style = 1;
                            client.Send(Serialize(dt));
                        }
                        if (check_name(message.tk, message.ten, message.id, 3) == false && check_name(message.email, "", message.id, 2) == false)
                        {
                            Create_Connect();
                            string sql = "Update Account set Taikhoan=@tk,Matkhau=@mk,Img=@img,Ngaysinh=@ns,Sex=@s,Ten=@ten,Email=@em  where Id=@id";
                            SqlCommand cmd = strConnect.CreateCommand();
                            cmd.CommandText = sql;
                            cmd.Parameters.Add("@tk", SqlDbType.NVarChar).Value = message.tk;
                            cmd.Parameters.Add("@mk", SqlDbType.NVarChar).Value = message.mk;
                            cmd.Parameters.Add("@img", SqlDbType.Image).Value = message.img;
                            cmd.Parameters.Add("@ns", SqlDbType.NVarChar).Value = message.ngaysinh;
                            cmd.Parameters.Add("@s", SqlDbType.NVarChar).Value = message.sex;
                            cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = message.ten;
                            cmd.Parameters.Add("@id", SqlDbType.Int).Value = message.id;
                            cmd.Parameters.Add("@em", SqlDbType.NVarChar).Value = message.email;
                            cmd.ExecuteNonQuery();
                            dt.tk = message.tk;
                            dt.email = message.email;
                            dt.ten = message.ten;
                            dt.style = 2;
                            dt.img = message.img;
                            client.Send(Serialize(dt));
                            strConnect.Close();
                        }
                    }
                    //Thêm trạng thái vào danh sách kết bạn
                    if (message.style == 6)
                    {
                        Create_Connect();
                        string sql = "Update Ketban set Readed=@rd where Nguoigui=@ng or Nguoinhan=@nn";
                        SqlCommand cmd = strConnect.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@rd", SqlDbType.Int).Value = 1;
                        cmd.Parameters.Add("@ng", SqlDbType.Int).Value = message.id_send;
                        cmd.Parameters.Add("@nn", SqlDbType.Int).Value = message.id_send;
                        cmd.ExecuteNonQuery();
                        strConnect.Close();
                    }
                    //Lấy tên,id của những người chưa có trong danh sách bạn và danh sách kết bạn
                    if (message.style == 7)
                    {
                        data dt = new data();
                        DataSet ds = new DataSet();
                        Create_Connect();
                        string sql = "select Account.Ten,Account.Id " +
                            "from Account " +
                            "where Ten like '" + message.msg + "%' " +
                            "and Account.Id<>'" + message.id_send + "'" +
                            "and (Account.Id not in (select Friend.Id_N from Friend,Account where '" + message.id_send + "'=Friend.Id_M and Account.Id=Friend.Id_N)) " +
                            "and (Account.Id not in (select Friend.Id_M from Friend,Account where '" + message.id_send + "'=Friend.Id_N and Account.Id=Friend.Id_M))" +
                            "and (Account.Id not in (select Ketban.Nguoigui from Ketban,Account where '" + message.id_send + "'=Ketban.Nguoinhan and Account.Id=Ketban.Nguoinhan and Ketban.Status='Wait'))" +
                            "and (Account.Id not in (select Ketban.Nguoinhan from Ketban, Account where '" + message.id_send + "' = Ketban.Nguoigui and Account.Id = Ketban.Nguoigui and Ketban.Status='Wait'))";
                        SqlDataAdapter adapter = new SqlDataAdapter(sql, strConnect);
                        adapter.Fill(ds, "banbe_id");
                        sql = "select Account.Ten " +
                            "from Account " +
                            "where Ten like '" + message.msg + "%' " +
                            "and Account.Id<>'" + message.id_send + "'" +
                            "and (Account.Id not in (select Friend.Id_N from Friend,Account where '" + message.id_send + "'=Friend.Id_M and Account.Id=Friend.Id_N)) " +
                            "and (Account.Id not in (select Friend.Id_M from Friend,Account where '" + message.id_send + "'=Friend.Id_N and Account.Id=Friend.Id_M))" +
                            "and (Account.Id not in (select Ketban.Nguoigui from Ketban,Account where '" + message.id_send + "'=Ketban.Nguoinhan and Account.Id=Ketban.Nguoinhan and Ketban.Status='Wait'))" +
                            "and (Account.Id not in (select Ketban.Nguoinhan from Ketban, Account where '" + message.id_send + "' = Ketban.Nguoigui and Account.Id = Ketban.Nguoigui and Ketban.Status='Wait'))";
                        adapter.SelectCommand.CommandText = sql;
                        adapter.Fill(ds, "banbe");
                        dt.ds = ds;
                        dt.style = 111;
                        client.Send(Serialize(dt));
                        strConnect.Close();
                    }
                    //Lấy thông tin trong bảng account
                    if (message.style == 8)
                    {
                        data dt = new data();
                        DataSet ds = new DataSet();
                        Create_Connect();
                        string sql = "select * from Account where Id='" + message.id_send + "'";
                        SqlDataAdapter adater = new SqlDataAdapter(sql, strConnect);
                        adater.Fill(ds, "Info");
                        dt.ds = ds;
                        dt.style = 100;
                        client.Send(Serialize(dt));
                        strConnect.Close();
                    }
                    //Chức năng thêm bạn, lấy danh sách kết bạn
                    if (message.style == 9)
                    {
                        data dt = new data();
                        dt.style = 1;
                        DataSet ds = new DataSet();
                        Create_Connect();
                        string sql = "insert into Ketban (Nguoigui,Nguoinhan,Status,Readed) values(@ng,@nn,@s,@r)";
                        SqlCommand cmd = strConnect.CreateCommand();
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@ng", SqlDbType.Int).Value = message.id_send;
                        cmd.Parameters.Add("@nn", SqlDbType.Int).Value = message.id_recv;
                        cmd.Parameters.Add("@s", SqlDbType.NVarChar).Value = "Wait";
                        cmd.Parameters.Add("@r", SqlDbType.Int).Value = 0;
                        cmd.ExecuteNonQuery();
                        sql = "select Ketban.Status,Ketban.Nguoigui,Ketban.Nguoinhan,Account.Ten,Ketban.Readed from Account,Ketban where Ketban.Nguoinhan='" + message.id_recv + "'and Ketban.Nguoigui=Account.Id or Ketban.Nguoigui='" + message.id_send + "' and Ketban.Nguoinhan=Account.Id and Ketban.Status='Accept'";
                        SqlDataAdapter adapter = new SqlDataAdapter(sql, strConnect);
                        adapter.Fill(ds, "ketban");
                        dt.ds = ds;
                        dt.id_recv = message.id_recv;
                        strConnect.Close();
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null)
                                    {
                                        item.Send(Serialize(dt));
                                    }
                                }
                            });
                        }
                        catch { }
                    }
                    //Chấp nhận kết bạn
                    if (message.style == 10)
                    {
                        data dt = new data();
                        DataSet ds = new DataSet();
                        dt.style = 2;
                        Create_Connect();
                        string sql = "Insert into Friend(Id_M,Id_N) values(@m,@n)";
                        SqlCommand cmd = new SqlCommand(sql, strConnect);
                        cmd.Parameters.Add("@m", SqlDbType.Int).Value = message.id_recv;
                        cmd.Parameters.Add("@n", SqlDbType.Int).Value = message.id_send;
                        cmd.ExecuteNonQuery();
                        sql = "Update Ketban set Status=@s,Readed=@r where Nguoigui=@ng and Nguoinhan=@nn and Status='Wait'"; ;
                        cmd.CommandText = sql;
                        cmd.Parameters.Add("@s", SqlDbType.NVarChar).Value = "Accept";
                        cmd.Parameters.Add("@r", SqlDbType.Int).Value = 0;
                        cmd.Parameters.Add("@ng", SqlDbType.Int).Value = message.id_send;
                        cmd.Parameters.Add("@nn", SqlDbType.Int).Value = message.id_recv;
                        cmd.ExecuteNonQuery();
                        Thread.Sleep(500);

                        sql = "select Ketban.Status,Ketban.Nguoigui,Ketban.Nguoinhan,Account.Ten,Ketban.Readed from Account,Ketban where Ketban.Nguoinhan='" + message.id_recv + "'and Ketban.Nguoigui=Account.Id or Ketban.Nguoigui='" + message.id_recv + "' and Ketban.Nguoinhan=Account.Id and Ketban.Status='Accept'";
                        SqlDataAdapter adapter = new SqlDataAdapter(sql, strConnect);
                        adapter.Fill(ds, "ketban1");
                        sql = "select Ketban.Status,Ketban.Nguoigui,Ketban.Nguoinhan,Account.Ten,Ketban.Readed from Account,Ketban where Ketban.Nguoinhan='" + message.id_send + "'and Ketban.Nguoigui=Account.Id or Ketban.Nguoigui='" + message.id_send + "' and Ketban.Nguoinhan=Account.Id and Ketban.Status='Accept'";
                        adapter.SelectCommand.CommandText = sql;
                        adapter.Fill(ds, "ketban2");
                        sql = "select Account.Ten,Account.Id,Account.Img,Account.Trangthai from Account,Friend where Friend.Id_M='" + message.id_send + "' and Friend.Id_N=Account.Id or Friend.Id_N='" + message.id_send + "' and Friend.Id_M=Account.Id";
                        adapter.SelectCommand.CommandText = sql;
                        adapter.Fill(ds, "ban1");
                        sql = "select Account.Ten,Account.Id,Account.Img,Account.Trangthai from Account,Friend where Friend.Id_M='" + message.id_recv + "' and Friend.Id_N=Account.Id or Friend.Id_N='" + message.id_recv + "' and Friend.Id_M=Account.Id";
                        adapter.SelectCommand.CommandText = sql;
                        adapter.Fill(ds, "ban2");
                        strConnect.Close();
                        dt.ds = ds;
                        dt.id_recv = message.id_send;
                        dt.id_send = message.id_recv;
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null)
                                    {
                                        item.Send(Serialize(dt));
                                    }
                                }
                            });
                        }
                        catch { }
                    }
                    //Từ chối kết bạn
                    if (message.style == 11)
                    {
                        Create_Connect();
                        string sql = "Update Ketban set Status=@s where Nguoigui=@ng and Status='Wait'";
                        SqlCommand cmd = new SqlCommand(sql, strConnect);
                        cmd.Parameters.Add("@s", SqlDbType.NVarChar).Value = "Refuse";
                        cmd.Parameters.Add("@ng", SqlDbType.Int).Value = message.id_send;
                        cmd.ExecuteNonQuery();
                        strConnect.Close();
                    }
                    //xoá bạn
                    if (message.style == 12)
                    {
                        data dt = new data();
                        DataSet ds = new DataSet();
                        dt.style = 3;
                        dt.id_send = message.id_send;
                        dt.id_recv = message.id_recv;
                        Create_Connect();
                        string sql = "Delete from Friend where (Id_M='" + message.id_recv + "' and Id_N='" + message.id_send + "') " +
                            "or (Id_N='" + message.id_recv + "' and Id_M='" + message.id_send + "')";
                        SqlCommand cmd = new SqlCommand(sql, strConnect);
                        cmd.ExecuteNonQuery();
                        sql = "select Account.Ten,Account.Id,Account.Img,Account.Trangthai from Account,Friend where Friend.Id_M='" + message.id_send + "' and Friend.Id_N=Account.Id or Friend.Id_N='" + message.id_send + "' and Friend.Id_M=Account.Id";
                        SqlDataAdapter adapter = new SqlDataAdapter(sql, strConnect);
                        adapter.Fill(ds, "ban1");
                        sql = "select Account.Ten,Account.Id,Account.Img,Account.Trangthai from Account,Friend where Friend.Id_M='" + message.id_recv + "' and Friend.Id_N=Account.Id or Friend.Id_N='" + message.id_recv + "' and Friend.Id_M=Account.Id";
                        adapter.SelectCommand.CommandText = sql;
                        adapter.Fill(ds, "ban2");
                        dt.ds = ds;
                        strConnect.Close();
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null)
                                    {
                                        item.Send(Serialize(dt));
                                    }
                                }
                            });
                        }
                        catch
                        {
                        }
                    }
                    //Tạo nhóm,add thành viên
                    if (message.style == 13)
                    {
                        int id_nhom = 0;
                        data dt = new data();
                        dt.style = 4;
                        if (check_name(message.msg, "", "", 1))
                        {
                            dt.msg = "fail";
                            client.Send(Serialize(dt));
                        }
                        else
                        {
                            dt.ngaysinh = "N', đã thêm bạn vào nhóm " + message.msg + "'";
                            DataSet ds = new DataSet();
                            Create_Connect();
                            string sql = "Insert into Nhom(Tennhom,Nguoitao) values(@ten,@nt)";
                            SqlCommand cmd = new SqlCommand(sql, strConnect);
                            cmd.Parameters.Add("@ten", SqlDbType.NVarChar).Value = message.msg;
                            cmd.Parameters.Add("@nt", SqlDbType.Int).Value = int.Parse(message.id);
                            cmd.ExecuteNonQuery();
                            sql = "select Id from Nhom where Tennhom='" + message.msg + "'";
                            cmd.CommandText = sql;
                            using (SqlDataReader rd = cmd.ExecuteReader())
                            {
                                if (rd.HasRows)
                                {
                                    while (rd.Read())
                                    {
                                        id_nhom = rd.GetInt32(0);
                                    }
                                }
                            }
                            DataTable dataTable = message.ds.Tables["Thanhvien"];
                            foreach (DataRow row in dataTable.Rows)
                            {
                                sql = "Insert into Thanhvien(Id_nhom,Id_account) values ('" + id_nhom + "','" + row["Id"] + "')";
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                                if (row["Id"].ToString() != message.id)
                                {
                                    sql = "Insert into Ketban (Nguoigui,Nguoinhan,Status,Readed) values (" + message.id + "," + row["Id"] + "," + dt.ngaysinh + "," + 0 + ")";
                                    cmd.CommandText = sql;
                                    cmd.ExecuteNonQuery();
                                }                     
                            }
                            dt.id = id_nhom.ToString();
                            dt.ten = message.msg;
                            dt.ds = message.ds;
                            dt.tk = message.ten;
                            dt.msg = "success";
                            strConnect.Close();
                            try
                            {
                                this.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    foreach (Socket item in clientList.ToList())
                                    {
                                        if (item != null)
                                        {
                                            item.Send(Serialize(dt));
                                        }
                                    }
                                });
                            }
                            catch
                            {

                            }
                        }
                    }
                    //cập nhật trạng thái on-off
                    if (message.style == 14)
                    {
                        data dt = new data();
                        dt.style = 5;
                        dt.msg = "off";
                        dt.id = message.id;
                        Create_Connect();
                        string sql = "update Account set Trangthai='off' where Id='" + message.id + "'";
                        SqlCommand cmd = new SqlCommand(sql, strConnect);
                        cmd.ExecuteNonQuery();
                        strConnect.Close();
                        Thread.Sleep(5000);
                        try
                        {
                            this.BeginInvoke((MethodInvoker)delegate ()
                            {
                                foreach (Socket item in clientList.ToList())
                                {
                                    if (item != null && item != client)
                                    {
                                        try
                                        {
                                            item.Send(Serialize(dt));
                                        }
                                        catch { }
                                    }
                                }
                            });
                        }
                        catch { }
                    }
                    if (message.style == 15)
                    {
                        data dt = new data();
                        Create_Connect();
                        dt.style = 999;
                        string sql = "select Matkhau from Account where Email='" + message.msg + "'";
                        SqlCommand cmd = new SqlCommand(sql, strConnect);
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.Read())
                        {
                            string pass = reader.GetString(0);
                            bool check = Send_Email(message.msg, Decrypt(pass));
                            if (check)
                            {
                                dt.msg = "true";
                                client.Send(Serialize(dt));
                            }
                            else
                            {
                                dt.msg = "false";
                                client.Send(Serialize(dt));
                            }
                        }
                        strConnect.Close();
                    }
                }
            }
            catch //(Exception e)
            {
                //MessageBox.Show(e.Message);
                clientList.Remove(client);
                //client.Close();
            }
        }
        public bool Send_Email(string email,string pass)
        {
            SmtpClient Client = new SmtpClient()
            {
                Host = "smtp.gmail.com",
                Port=587,
                EnableSsl=true,
                DeliveryMethod=SmtpDeliveryMethod.Network,
                UseDefaultCredentials=false,
                Credentials =new NetworkCredential()
                {
                    UserName="kanekirito1278@gmail.com",
                    Password="1277624312"
                }
            };
            MailAddress FromEmail = new MailAddress("kanekirito1278@gmail.com", "App_Chat");
            MailAddress ToEmail = new MailAddress(email, "someone");
            MailMessage message = new MailMessage()
            {
                From = FromEmail,
                Subject = "Your password in application chat!",
                Body = pass,
            };
            message.To.Add(ToEmail);
            try
            {
                Client.Send(message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool check_name(string s, string t,string id,int signal)
        {
            Create_Connect();
            string sqll="";
            if (signal==0)
            {
                sqll = "select Taikhoan from Account where Taikhoan='" + s + "'or Ten='" + t + "'";
            }    
            if (signal==1)
            {
                sqll = "select Tennhom from Nhom where Tennhom='" + s + "'";
            }   
            if (signal==2)
            {
                sqll = "select Email from Account where Email='" + s + "' and Id<>'"+id+"'" ;
            }  
            if (signal==3)
            {
                sqll = "select Taikhoan from Account where (Taikhoan='" + s + "'or Ten='" + t + "') and Id<>'" + id + "'";
            }
            SqlCommand cmd = new SqlCommand(sqll, strConnect);
            SqlDataReader dta = cmd.ExecuteReader();
            if (dta.Read() == true)
            {
                strConnect.Close();
                //strConnect.Dispose();
                return true;
            }
            strConnect.Close();
            //strConnect.Dispose();
            return false;

        }
        //add mesage vào khung chat
        void AddMessage(string s)
        {
            lsvMessage.Items.Add(new ListViewItem() { Text = s });
        }

        //Hàm phân mảnh dữ liệu cần gửi từ dạng string sang dạng byte để gửi đi
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
        //gửi tin cho nhiều client   
        private void button1_Click_1(object sender, EventArgs e)
        {
            foreach (Socket item in clientList)
            {
                Send(item);
            }
            AddMessage(txtMessage.Text);
            txtMessage.Clear();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Close();
            //server.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Connect();
            button2.Enabled = false;
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
    }
}
