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
    public partial class Image_show : UserControl
    {
        public Image_show()
        {
            InitializeComponent();
        }
        public Image_show(Image img)
        {
            this.Size = new Size(391, 292);
            int h = img.Height;
            int w = img.Width;
            if (h<this.Height&&w<this.Width)
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
        System.Drawing.Image resize(System.Drawing.Image img, int w, int h)
        {
            Bitmap bmp = new Bitmap(w, h);
            Graphics graphics = Graphics.FromImage(bmp);
            graphics.DrawImage(img, 0, 0, w, h);
            graphics.Dispose();
            return bmp;
        }
    }
}
