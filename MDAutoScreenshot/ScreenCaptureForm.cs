using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MDAutoScreenshot
{
    public partial class ScreenCaptureForm : Form
    {
        public MDatScreenshot mainform;
        public ScreenCaptureForm()
        {
            InitializeComponent();
            this.MouseDown += ScreenCaptureForm_MouseDown;
            this.MouseUp += ScreenCaptureForm_MouseUp;
            this.MouseMove += ScreenCaptureForm_MouseMove;
        }

        private void ScreenCaptureForm_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.Opacity = 0.5;
        }
        private Point startPoint;
        private Point endPoint;
        private bool isMousePressed;

        public Rectangle SelectedRegion
        {
            get { return new Rectangle(startPoint.X, startPoint.Y, endPoint.X - startPoint.X, endPoint.Y - startPoint.Y); }
        }

        private void ScreenCaptureForm_MouseDown(object sender, MouseEventArgs e)
        {
            startPoint = e.Location;
            endPoint = startPoint;
            isMousePressed = true;
        }
        private void ScreenCaptureForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMousePressed)
            {
                endPoint = e.Location;
                this.Invalidate(); // Redraw the form to update the selected region
            }
        }
        private void ScreenCaptureForm_MouseUp(object sender, MouseEventArgs e)
        {
            endPoint = e.Location;
            DialogResult = DialogResult.OK; 
            Rectangle selectedRect = new Rectangle(
                 Math.Min(startPoint.X, endPoint.X),
                 Math.Min(startPoint.Y, endPoint.Y),
                 Math.Abs(endPoint.X - startPoint.X),
                 Math.Abs(endPoint.Y - startPoint.Y));

            // Ví dụ: vẽ một hình chữ nhật để biểu diễn khu vực đã chọn trên form
            using (Graphics g = this.CreateGraphics())
            {
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    g.DrawRectangle(pen, selectedRect);
                }
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (startPoint != Point.Empty && endPoint != Point.Empty)
            {
                int width = Math.Abs(endPoint.X - startPoint.X);
                int height = Math.Abs(endPoint.Y - startPoint.Y);
                Rectangle selectedRect = new Rectangle(Math.Min(startPoint.X, endPoint.X), Math.Min(startPoint.Y, endPoint.Y), width, height);
                using (Pen pen = new Pen(Color.Red, 2))
                {
                    e.Graphics.DrawRectangle(pen, selectedRect);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
