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
        public Banbe()
        {
            InitializeComponent();
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
                guna2CirclePictureBox1.Image = Image.FromStream(mem);
                guna2CirclePictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
                guna2CirclePictureBox1.Visible = false;
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
                no.addcontrols(Id_ban, 0);
            }    
            if (phanloai==0)
            {
                showchatbox(Id_ban);
            } 
            if (phanloai==2)
            {
                //guna2CirclePictureBox1.Visible = false;
            }    
        }
        public void usercontrol(UserControl userControl)
        {
            control = userControl;
        }
        public void showchatbox(string s)
        {
            Danhsach_tinnhan tam= control as Danhsach_tinnhan;
            tam.showchatbox(s,image,guna2Button1.Text);
        }
    }
}
