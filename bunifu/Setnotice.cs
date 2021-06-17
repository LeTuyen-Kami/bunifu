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
    public partial class Setnotice : UserControl
    {
        public Setnotice()
        {
            InitializeComponent();
        }
        public void set(DataTable dataTable,string id)
        {
            for (int i=dataTable.Rows.Count-1;i>=0;i--)
            {
                string value_row_nguoigui = dataTable.Rows[i]["Nguoigui"].ToString();
                string value_row_nguoinhan = dataTable.Rows[i]["Nguoinhan"].ToString();
                int value_readed = (int)dataTable.Rows[i]["Readed"];
                string value_status = dataTable.Rows[i]["Status"].ToString();
                string value_ten = dataTable.Rows[i]["Ten"].ToString();
                if (value_row_nguoigui==id)
                {
                    Noctice noctice = new Noctice();
                    noctice.setid(value_row_nguoigui, value_row_nguoinhan);
                    noctice.Addnotice(value_ten,", đã chấp nhận lời mời kết bạn" ,value_readed,1);
                    noctice.Size = noctice1.Size;
                    noctice.Dock = DockStyle.Top;
                    this.Controls.Add(noctice);
                    noctice.BringToFront();
                }    
                if (value_row_nguoinhan==id)
                {
                    Noctice noctice = new Noctice();
                    noctice.ten(value_ten);
                    noctice.setid(value_row_nguoigui, value_row_nguoinhan);
                    if (value_status=="Wait")
                    {
                        noctice.Addnotice(value_ten, ", đã gửi lời mời kết bạn", value_readed,0);
                    }
                    if (value_status=="Accept")
                    {
                        noctice.Addnotice("Bạn", ", đã chấp nhận lời mời kết bạn của "+value_ten, value_readed,1);
                    }
                    if (value_status == "Refuse")
                    {
                        noctice.Addnotice("Bạn", ", đã từ chối lời mời kết bạn của " + value_ten, value_readed,1);
                    }
                    noctice.Size = noctice1.Size;
                    noctice.Dock = DockStyle.Top;
                    this.Controls.Add(noctice);
                    noctice.BringToFront();
                }    
            }    
             
        }

        private void noctice1_Load(object sender, EventArgs e)
        {

        }
    }
}
