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
    public partial class Thanhvien : UserControl
    {
        string Id;
        public Thanhvien()
        {
            InitializeComponent();
        }
        public void nhap(string s,string id)
        {
            label1.Text = s;
            button1.Location = new Point(label1.Width + 5, button1.Location.Y);
            panel1.Size = new Size(label1.Width + button1.Width+5, panel1.Height);
            Id = id;
           
        }

        private void Thanhvien_Load(object sender, EventArgs e)
        {
            var bm = new Bitmap(button1.Image, new Size(25, 25));
            button1.Image = bm;
            button1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Taonhom taonhom = (Taonhom)(this.ParentForm);
            taonhom.remove_form(this,label1.Text,Id);
        }

        private void panel1_MouseHover(object sender, EventArgs e)
        {
            button1.Visible = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            button1.Visible = false;
        }

        private void button1_MouseHover(object sender, EventArgs e)
        {
            button1.Visible = true;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
        }

        private void label1_MouseHover(object sender, EventArgs e)
        {
            button1.Visible = true;
        }

        private void label1_MouseLeave(object sender, EventArgs e)
        {
        }
    }
}
