using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bunifu
{
    public partial class Noctice : UserControl
    {
        ClienT client = new ClienT();
        string Myid;
        string Yourid;
        string name;
        public Noctice()
        {
            InitializeComponent();
        }
        public void Addnotice(string dl1, string dl2, int a, int b)
        {
            label1.Text = dl1;
            label2.Location = new Point(label1.Location.Y + label1.Width - 10, label1.Location.Y);
            label2.Text = dl2;
            if (a == 1)
            {
                this.BackColor = Color.Gray;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
                label2.Font = new Font(label2.Font, FontStyle.Regular);
            }
            if (b == 1)
            {
                guna2Button1.Visible = false;
                guna2Button2.Visible = false;
            }
        }
        private void Noctice_Load(object sender, EventArgs e)
        {
            client.Connect();
        }
        public void setid(string id_send, string id_recv)
        {
            Myid = id_recv;
            Yourid = id_send;
        }
        public void ten(string s)
        {
            name = s;
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            data dt = new data();
            dt.style = 10;
            dt.id_recv =int.Parse(Myid);
            dt.id_send =int.Parse(Yourid);
            client.Send(dt);
            // no = (none)(this.ParentForm);
            //no.addrow_datafriend(label1.Text,int.Parse(Yourid));
            Addnotice("Bạn", ", đã chấp nhận lời mời kết bạn của "+name, 1, 1);
        }
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Addnotice("Bạn", ", đã từ chối lời mời kết bạn của "+name, 1, 1);
            data dt = new data();
            dt.style = 11;
            dt.id_recv = int.Parse(Myid);
            dt.id_send = int.Parse(Yourid);
            client.Send(dt);
            
            none no = (none)(this.ParentForm);
            no.UpdateKetban(Yourid);
        }
    }
}
