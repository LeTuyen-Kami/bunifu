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
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
        }
        public About(Color color)
        {
            InitializeComponent();
            label2.BackColor = color;
            label1.Text = "Có thể bạn chưa biết:\n" +
            "Ứng dụng MS Store trên Windows 11 mới được viết hoàn toàn bằng XAML / C# \n" +
            "thay vì sử dụng một phần nội dung hiển thị web như trước đây. Microsoft đã tuyển những kỹ sư tài năng nhất\n" +
            "để phát triển lại ứng dụng này để mang đến trải nghiệm tốt hơn cho các nhà phát triển và người dùng.\n" +
            "Họ đã làm việc bí mật trong suốt 1 năm, do đó chúng ta không hề thấy bất kỳ hình ảnh, thông tin nào về ứng dụng mới này trong suốt thời gian qua.\n" +
            "MS Store mới sẽ hỗ trợ app Android. Tuy nhiên, để sử dụng được cài đặt được app Android thì bạn phải cài đặt Amazon App Store trước(hơi tốn công một tí).\n" +
            "MS Store mới sẽ hỗ trợ hầu hết các loại dứng dụng phổ biến hiện nay: Win32, .NET, UWP, Xamarin, Electron, React Native, Java and even Progressive Web Apps.\n" +
            "P / s: Cái store mới nhìn giống của MacOS thế 😵\n" +
            "Và còn nhiều thông tin khác, các bạn có thể xem thêm tại:";
        }
        private void guna2RatingStar1_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
