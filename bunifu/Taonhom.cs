using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace bunifu
{
    public partial class Taonhom : Form
    {
        DataTable dataTable;
        DataTable data_send_server = new DataTable();
        DataTable dataid;
        DataTable data;
        public Taonhom()
        {

            this.EnableBlur();
            BackColor = Color.Azure;
            TransparencyKey = Color.Azure;
            InitializeComponent();
        }
        public void nhap(DataTable dt)
        {
            dataTable = dt;
        }
        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        public void remove_form(Thanhvien thanhvien,string ten,string id)
        {
            panel2.Controls.Remove(thanhvien);
            data.Rows.Add(ten);
            dataid.Rows.Add(id);
            dataTable.Rows.Add(new object[] { ten,Int32.Parse(id) });
            for (int i=data_send_server.Rows.Count-1;i>=0;i--)
            {
                if (data_send_server.Rows[i]["Id"].ToString() == id)
                    data_send_server.Rows.RemoveAt(i);
            }
            guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                            (guna2DataGridView1.RowCount) * guna2DataGridView1.RowTemplate.Height + 5);
  
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
      
        }

        private void guna2TextBox2_TextChanged(object sender, EventArgs e)
        {
            if (guna2TextBox2.Text!="")
            {
                data = new DataTable();
                dataid = new DataTable();
                data.Columns.Add("Ten");
                dataid.Columns.Add("Id");                
                foreach (DataRow row in dataTable.Rows)
                {
                    string Value_of_rowTen = row["Ten"].ToString().ToLower();
                    if (Value_of_rowTen.Contains(guna2TextBox2.Text.ToLower()))
                    {
                        data.Rows.Add(row["Ten"].ToString());
                        string tam = row["Id"].ToString();
                        dataid.Rows.Add(tam);
                    }
                }
                guna2DataGridView1.DataSource = data;
                guna2DataGridView1.Visible = true;
                guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                            (guna2DataGridView1.RowCount) * guna2DataGridView1.RowTemplate.Height + 5);
            }    
            else
            {
                guna2DataGridView1.Visible = false;
            }    

        }
        private void Taonhom_Load(object sender, EventArgs e)
        {
            data_send_server.Columns.Add("Id");
        }

        private void guna2DataGridView1_CellDoubleClick_1(object sender, DataGridViewCellEventArgs e)
        {
            Thanhvien thanhvien = new Thanhvien();
            int position_of_row = e.RowIndex;
            
            string id = dataid.Rows[position_of_row]["Id"].ToString();
            thanhvien.nhap(guna2DataGridView1[e.ColumnIndex, position_of_row].Value.ToString(), id);
            data_send_server.Rows.Add(id);
            data.Rows.RemoveAt(position_of_row);
            
            for (int i=dataTable.Rows.Count-1; i>=0;i--)
            {
                string tam1 = dataTable.Rows[i]["Id"].ToString();
                if (tam1==id )
                {
                    dataTable.Rows.RemoveAt(i);
                }    
            }
            dataid.Rows.RemoveAt(position_of_row);
            //guna2DataGridView1.Rows.RemoveAt(position_of_row);
            guna2DataGridView1.Size = new Size(guna2DataGridView1.Width,
                            (guna2DataGridView1.RowCount) * guna2DataGridView1.RowTemplate.Height + 5);
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    if (row["Id"].ToString() == dataid.Rows[e.RowIndex]["Id"].ToString())
            //    {
            //        dataTable.Rows.Remove(row);
            //    }
            //}
            panel2.Controls.Add(thanhvien);
            thanhvien.Dock = DockStyle.Top;
        }
    }
    public static class WindowExtension
    {
        [DllImport("user32.dll")]
        static internal extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        public static void EnableBlur(this Form @this)
        {
            var accent = new AccentPolicy();
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var Data = new WindowCompositionAttributeData();
            Data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            Data.SizeOfData = accentStructSize;
            Data.Data = accentPtr;
            SetWindowCompositionAttribute(@this.Handle, ref Data);
            Marshal.FreeHGlobal(accentPtr);
        }

    }
    enum AccentState
    {
        ACCENT_ENABLE_BLURBEHIND = 3
    }

    struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    enum WindowCompositionAttribute
    {
        WCA_ACCENT_POLICY = 19
    }
}
