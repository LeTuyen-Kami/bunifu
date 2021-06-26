using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.Common;
using Bunifu.UI.WinForms;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace bunifu
{
    public partial class Login : Form
    {
        LoginC login = new LoginC();
        Signup signup = new Signup();
        public Login()
        {
            InitializeComponent();
        }   
        private const int drop = 0x00020000;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = drop;
                return cp;
            }
        } 
        private void bunifuDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //bunifuTextBox1.Text = bunifuDataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
            //bunifuDataGridView1.Visible = false;
        }
        Color a = Color.FromArgb(40, 141, 252);
        Color b = Color.FromArgb(249, 24, 151);
        private void bunifuGradientPanel1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {
            panel2.Controls.Add(login);
            //this.Cursor=new Cursor(Application.StartupPath+"\\")
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        public void click()
        {
            login.Dock = DockStyle.Fill;
            panel2.Controls.Add(login);
            login.BringToFront();
            bunifuGradientPanel1.GradientBottomRight = a;
            bunifuGradientPanel1.GradientTopLeft = b;
            a = bunifuGradientPanel1.GradientTopLeft;
            b = bunifuGradientPanel1.GradientBottomRight;
        }
        private void guna2Button1_Click(object sender, EventArgs e)
        {
            login.Dock = DockStyle.Fill;
            panel2.Controls.Add(login);
            login.BringToFront();
            bunifuGradientPanel1.GradientBottomRight = a;
            bunifuGradientPanel1.GradientTopLeft = b;
            a = bunifuGradientPanel1.GradientTopLeft;
            b = bunifuGradientPanel1.GradientBottomRight;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            signup.Dock = DockStyle.Fill;
            panel2.Controls.Add(signup);
            signup.BringToFront();
            bunifuGradientPanel1.GradientBottomRight = a;
            bunifuGradientPanel1.GradientTopLeft = b;
            a = bunifuGradientPanel1.GradientTopLeft;
            b = bunifuGradientPanel1.GradientBottomRight;
        }
        public void Forgot_pass()
        {
            Forgot_pass forgot_Pass = new Forgot_pass();
            forgot_Pass.Dock = DockStyle.Fill;
            panel2.Controls.Add(forgot_Pass);
            forgot_Pass.BringToFront();
        }
    }
}
