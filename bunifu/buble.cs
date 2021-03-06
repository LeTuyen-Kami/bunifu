using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace bunifu
{
    public partial class buble : UserControl
    {
        byte[] Data_file;
        public buble()
        {
            InitializeComponent();
        }
        public buble(int i)
        {
            InitializeComponent();
            this.Size = new Size(42,43);
            label1.Visible = false;
            lbltime.Visible = false;
            this.BackgroundImage = new Bitmap(@"like.png");
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.BackColor = Color.Transparent;
        }
        public buble(string s,string time,byte[] data,msgtype messagetype)
        {
            InitializeComponent();
            Data_file = data;
            label1.Text = s;
            lbltime.Text = time;
            label1.Cursor = Cursors.Hand;
            label1.Font = new Font(label1.Font.Name, 12, FontStyle.Underline);
            if (messagetype.ToString() == "In")
            {
                this.BackColor = Color.Gray;
            }
            else
            {
                this.BackColor = Color.FromArgb(213, 67, 183);
            }
            label1.Click += new EventHandler(label_click);
            setheight();
        }
        private void label_click(object sender,EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = label1.Text;
            sfd.ShowDialog();
            File.WriteAllBytes(sfd.FileName, Data_file);
        }
        public buble(Image img)
        {
            InitializeComponent();
            label1.Visible = false;
            lbltime.Visible = false;
            this.Size = new Size(391, 292);
            int h = img.Height;
            int w = img.Width;
            if (h < this.Height && w < this.Width)
            {
                this.Size = new Size(w, h);
                this.BackgroundImage = img;
            }
            else
            {
                this.Size = new Size(this.Width, (int)((this.Width * h) / w));
                this.BackgroundImage = resize(img, this.Width, this.Height);
            }
        }
        public buble(string message, string time, msgtype messagetype)
        {
            InitializeComponent();
            label1.Text = message;
            lbltime.Text = time;
            if (messagetype.ToString() == "In")
            {
                this.BackColor = Color.Gray;
            }
            else
            {
                this.BackColor = Color.FromArgb(213, 67, 183);
            }
            setheight();
        }
        public enum msgtype
        {
            In,
            Out
        }
        void setheight()
        {
            Graphics g = CreateGraphics();
            SizeF size = g.MeasureString(label1.Text, label1.Font, label1.Width);
            label1.Height = int.Parse(Math.Round(size.Height + 10, 0).ToString());
            Size sizef = TextRenderer.MeasureText(label1.Text, label1.Font);
            if (sizef.Width<300-5)
            {
                if (sizef.Width>lbltime.Width)
                {
                    label1.Width = sizef.Width + 15;
                    this.Width = sizef.Width + 30;
                }    
                else
                {
                    label1.Width = lbltime.Width;
                    this.Width = lbltime.Width + 20;
                }    
            }    

            lbltime.Top = label1.Bottom;
            this.Height = lbltime.Bottom + 10;
        }
        System.Drawing.Image resize(System.Drawing.Image img, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.DrawImage(img, 0, 0, w, h);
            graphics.Dispose();
            return bmp;
        }
        private void lbltime_Resize(object sender, EventArgs e)
        {
            //if (label1.Width <= lbltime.Width) this.Width = lbltime.Width+40;
            //else this.Width = label1.Width + 15;
            //this.Height = label1.Height + lbltime.Height + 15;
            //setheight();
        }

        private void buble_Resize(object sender, EventArgs e)
        {
            //setheight();
        }
    }
}
