using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDAutoScreenshot
{
    public partial class MDatScreenshot : Form
    {
        private FolderBrowserDialog folderBrowserDialog;
        public MDatScreenshot()
        {
            InitializeComponent();
            notifyIcon = new NotifyIcon();
            notifyIcon.Icon = this.Icon;
            notifyIcon.Visible = true;
            notifyIcon.Text = "Application Name";
            notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            // Khởi tạo FolderBrowserDialog
            folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Select the save location.";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //this.Text = "Screenshot Auto from ManhDat";
        }
        private bool isSelectedRegion = false;
        private Rectangle selectedRegion;

        private void selectRegionButton_Click(object sender, EventArgs e)
        {
            // Hiển thị hộp thoại thông báo cho phép người dùng chọn khu vực trên màn hình
            MessageBox.Show("Kéo chọn vùng mà anh bạn muốn chụp đi.", "Select Region", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Hide();
            // Đợi cho người dùng chọn khu vực trên màn hình
            using (var screenCaptureForm = new ScreenCaptureForm())
            {
                screenCaptureForm.mainform = this;

                if (screenCaptureForm.ShowDialog() == DialogResult.OK)
                {
                    if (screenCaptureForm.SelectedRegion.Width > 0 && screenCaptureForm.SelectedRegion.Height > 0)
                    {
                        selectedRegion = screenCaptureForm.SelectedRegion;
                        MessageBox.Show($"Đây nhé chọn vùng này: {selectedRegion}", "Chọn vùng xong rồi đấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        label5.Text = "Vùng chọn: " + selectedRegion;
                        isSelectedRegion = true;
                        Show();
                    }
                    else
                    {
                        MessageBox.Show("Vùng mà anh bạn chọn ko hợp lệ", "Screenshot Captured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Show();
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.P))
            {
                ScreenshotAction();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void ScreenshotAction()
        {
            if (isSelectedRegion)
            {
                // Chụp màn hình trong khu vực đã chọn và lưu ảnh
                string savepath = path;
                if (!string.IsNullOrEmpty(textBox1.Text))
                {
                    savepath = textBox1.Text;
                }
                else
                {
                    Focus();
                    MessageBox.Show("Chọn Save Folder đi đã anh bạn", "Screenshot Captured", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Bitmap screenshot = CaptureScreen(selectedRegion);
                screenshot.Save(savepath + @"\screenshot" + DateTime.Now.Ticks + ".png");
                ShowNotification("Chụp được hình và lưu lại rồi nhá.");

            }
            else
            {
                Focus();
                MessageBox.Show("Chọn vùng đã rồi tôi mới biết chụp ở đâu chứ anh bạn.", "No Region Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        string path = @"C:\Users\YourUsername\Documents";
        private Bitmap CaptureScreen(Rectangle region)
        {
            Bitmap bitmap = new Bitmap(region.Width, region.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(region.Location, Point.Empty, region.Size);
            }
            return bitmap;
        }
        
        private void Form1_Load_1(object sender, EventArgs e)
        {
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            selectRegionButton_Click(sender, e);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }


        private NotifyIcon notifyIcon; 
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            // Xử lý sự kiện khi người dùng double-click vào biểu tượng NotifyIcon
            // Ví dụ: Hiển thị cửa sổ chính của ứng dụng
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void ShowNotification(string message)
        {
            notifyIcon.BalloonTipTitle = "Hello anh bạn";
            notifyIcon.BalloonTipText = message;
            notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            notifyIcon.ShowBalloonTip(1000); // Hiển thị thông báo trong 2 giây
        }



        private const int WM_HOTKEY = 0x0312;
        private const int MOD_ALT = 0x0001;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_HOTKEY)
            {
                // Kiểm tra xem phím tắt có phải là Alt + P không
                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                if (key == Keys.P && ModifierKeys == Keys.Alt)
                {
                    // Thực hiện hành động khi phím tắt được nhấn
                    ScreenshotAction();
                }
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Đăng ký phím tắt Alt + P
            RegisterHotKey(this.Handle, 0, MOD_ALT, (int)Keys.P);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Hủy đăng ký phím tắt khi form đóng
            UnregisterHotKey(this.Handle, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {// Mở FolderBrowserDialog để chọn vị trí lưu trữ
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                // Lấy đường dẫn đã chọn và hiển thị trong TextBox hoặc thực hiện các hành động khác
                string selectedPath = folderBrowserDialog.SelectedPath;
                textBox1.Text = selectedPath;
            }
        }
    }
}

