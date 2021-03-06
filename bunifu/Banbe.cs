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
using Guna.UI2.WinForms;

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
        string Trangthai;
        public Banbe()
        {
            InitializeComponent();
        }
        public Banbe(Color color)
        {
            InitializeComponent();
            Tile_color = color;
        }
        public void Addten(string s,string Id,byte[] img,string my_id,int loai,string trangthai)
        {
            guna2Button1.Text = s;
            Id_M = my_id;
            Id_ban = Id;
            phanloai = loai;
            Trangthai = trangthai;
            if (loai != 2)
            {
                image = img;
                MemoryStream mem = new MemoryStream(img);
                guna2CirclePictureBox1.Image =System.Drawing.Image.FromStream(mem);
                guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                if (trangthai=="off")
                {
                    guna2CirclePictureBox2.FillColor = Color.Gray;
                }    
                if (trangthai=="on")
                {
                    guna2CirclePictureBox2.FillColor = Color.Green;
                }    
            }
            else
            {
                guna2CirclePictureBox2.Visible = false;
                guna2CirclePictureBox1.Visible = false;
                guna2Button1.TextAlign = HorizontalAlignment.Right;
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
                tam.showchatbox(Id_ban, image, guna2Button1.Text,"0",Trangthai,guna2Button1.Text);
            } 
            if (phanloai==2)
            {
                Danhsach_tinnhan tam = control as Danhsach_tinnhan;
                tam.showchatbox(Id_ban, image, guna2Button1.Text, "1",Trangthai,guna2Button1.Text);
            }    
        }
        public void usercontrol(UserControl userControl)
        {
            control = userControl;
        }

        private void guna2Button1_MouseDown(object sender, MouseEventArgs e)
        {           
        }
    }
}
