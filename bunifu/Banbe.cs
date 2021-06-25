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
    public partial class Banbe : UserControl
    {
        string Id_M;
        string Id_ban;
        int phanloai;
        byte[] image;
        UserControl control;
        Color Tile_color;
        public Banbe()
        {
            InitializeComponent();
        }
        public Banbe(Color color)
        {
            InitializeComponent();
            Tile_color = color;
        }
        public void Addten(string s,string Id,byte[] img,string my_id,int loai)
        {
            guna2Button1.Text = s;
            Id_M = my_id;
            Id_ban = Id;
            phanloai = loai;
            if (loai != 2)
            {
                image = img;
                MemoryStream mem = new MemoryStream(img);
                guna2CirclePictureBox1.Image =System.Drawing.Image.FromStream(mem);
                guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                guna2CirclePictureBox1.Visible = false;
                guna2Button1.TextAlign = HorizontalAlignment.Center;
            }
        }
        public void Esclip()
        {
            guna2Button1.AutoRoundedCorners = true;
            guna2Button1.CustomizableEdges.TopLeft = false;
            guna2Button1.CustomizableEdges.TopRight = false;
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            if (phanloai==1)
            {
                none no = (none)(this.ParentForm);
                no.addcontrols(Id_ban, 0,Tile_color);
            }    
            if (phanloai==0)
            {
                Danhsach_tinnhan tam = control as Danhsach_tinnhan;
                tam.showchatbox(Id_ban, image, guna2Button1.Text,"0");
            } 
            if (phanloai==2)
            {
                Danhsach_tinnhan tam = control as Danhsach_tinnhan;
                tam.showchatbox(Id_ban, image, guna2Button1.Text, "1");
            }    
        }
        public void usercontrol(UserControl userControl)
        {
            control = userControl;
        }
    }
}
