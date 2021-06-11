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
        public Noctice()
        {
            InitializeComponent();
        }
        public void Addnotice(string dl1,string dl2,int a,int b)
        {
            label1.Text = dl1;
            label2.Location = new Point(label1.Location.Y+label1.Width-10, label1.Location.Y);
            label2.Text = dl2;
            if (a==1)
            {
                this.BackColor = Color.Gray;
                label1.Font = new Font(label1.Font, FontStyle.Regular);
                label2.Font = new Font(label2.Font, FontStyle.Regular);
            } 
            if (b==1)
            {
                guna2Button1.Visible = false;
                guna2Button2.Visible = false;
            }    
        }
        private void Noctice_Load(object sender, EventArgs e)
        {

        }
    }
}
