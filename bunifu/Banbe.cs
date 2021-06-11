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
    public partial class Banbe : UserControl
    {
        string id;
        public Banbe()
        {
            InitializeComponent();
        }
        public void Addten(string s,string Id)
        {
            guna2Button1.Text = s;
            id = Id;
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            none no = (none)(this.ParentForm);
            no.addcontrols(id);
        }
    }
}
