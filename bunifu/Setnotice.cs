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
            foreach(DataRow row in dataTable.Rows)
            {
                if (row["Nguoigui"].ToString()==id)
                {
                    Noctice noctice = new Noctice();
                    noctice.Addnotice(row["Ten"].ToString(),", đã chấp nhận lời mời kết bạn" ,(int)row["Readed"],1);
                    noctice.Size = noctice1.Size;
                    noctice.Dock = DockStyle.Bottom;
                    this.Controls.Add(noctice);
                    this.Controls.Add(noctice);
                    this.Controls.Add(noctice);
                    this.Controls.Add(noctice);
                    this.Controls.Add(noctice);
                    this.Controls.Add(noctice);
                    noctice.BringToFront();
                }    
                if (row["Nguoinhan"].ToString()==id)
                {
                    Noctice noctice = new Noctice();
                    noctice.Addnotice(row["Ten"].ToString(),", đã gửi lời mời kết bạn", (int)row["Readed"], 0);
                    noctice.Size = noctice1.Size;
                    noctice.Dock = DockStyle.Bottom;
                    this.Controls.Add(noctice);
                    noctice.BringToFront();
                }    
            }    
             
        }
    }
}
