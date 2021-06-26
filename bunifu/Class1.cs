using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bunifu
{
    [Serializable]
    public class data
    {
        public string ten = "";
        public string msg = "";
        public int style = 0;
        public string tk = "";
        public string mk = "";
        public int id_send=0;
        public int id_recv=0;
        public string time="";
        public DataSet ds=new DataSet();
        public byte[] img=new byte[1];
        public string ngaysinh = "";
        public string ngaytao = "";
        public string sex = "";
        public string id = "";
        public string loai_mes = "0";
        public string loai_nhan = "0";
        public byte[] image=new byte[1];
        public string email = "";
    }
}
