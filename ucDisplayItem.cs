using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong
{
    public partial class ucDisplayItem : UserControl
    {
        public string ID { get; set; } = "";
        public int STT { get; set; } = 0;
        public static Color currentColor = Color.LightGray;

        public ucDisplayItem()
        {
            InitializeComponent();
        }
        public ucDisplayItem(int STT, string ServiceCode, string ChinaPlateNum, string VietNamPlateNum, string Position, string Group, EM_ParkingStatus status)
        {
            InitializeComponent();
            DisplayData(STT, ServiceCode, ChinaPlateNum, VietNamPlateNum, Position, Group, status);
            lblSTT.BackColor = currentColor;
            lblSTT.ForeColor = Color.Black;

            lblServiceCode.BackColor = currentColor;
            lblServiceCode.ForeColor = Color.Black;

            lblChinaPlateNumber.BackColor = currentColor;
            lblChinaPlateNumber.ForeColor = Color.DarkRed;

            lblVietNamPlateNumber.BackColor = currentColor;
            lblVietNamPlateNumber.ForeColor = Color.DarkGreen;

            lblPosition.BackColor = currentColor;
            lblPosition.ForeColor = Color.Black;

            lblGroup.BackColor = currentColor;
            lblGroup.ForeColor = Color.Black;

            panel1.BackColor = currentColor;
            panel1.ForeColor = Color.Black;

            currentColor = currentColor == Color.LightGray ? Color.FromArgb(191, 187, 186) : Color.LightGray;
            //var path = new System.Drawing.Drawing2D.GraphicsPath();
            //path.AddEllipse(0, 0, lblStatus.Width, lblStatus.Height,);

            this.lblStatus.Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(10, 10, this.lblStatus.Width-10, this.lblStatus.Height-10, 40, 40));
        }
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );

        private void DisplayData(int STT, string ServiceCode, string ChinaPlateNum, string VietNamPlateNum, string Position, string Group, EM_ParkingStatus status)
        {
            lblSTT.Text = STT.ToString();

            lblServiceCode.Text = ServiceCode;
            lblChinaPlateNumber.Text = ChinaPlateNum;
            lblVietNamPlateNumber.Text = VietNamPlateNum;
            lblPosition.Text = Position;
            lblGroup.Text = Group;
            lblStatus.Text = StaticPool.GetStatusName(status);
            switch (status)
            {
                case EM_ParkingStatus.CHUA_XAC_NHAN:
                case EM_ParkingStatus.UNKNOWN:
                    lblStatus.BackColor = Color.Maroon;
                    lblStatus.ForeColor = Color.White;
                    break;
                case EM_ParkingStatus.CHUA_PHAN_TO:
                    lblStatus.BackColor = Color.DarkRed;
                    lblStatus.ForeColor = Color.White;
                    break;
                case EM_ParkingStatus.DA_PHAN_TO:
                    lblStatus.BackColor = Color.Gold;
                    lblStatus.ForeColor = Color.White;
                    break;
                case EM_ParkingStatus.DANG_THUC_HIEN:
                    lblStatus.BackColor = Color.FromArgb(230, 174, 32);
                    lblStatus.ForeColor = Color.White;
                    break;
                case EM_ParkingStatus.CHO_DUYET:
                    lblStatus.BackColor = Color.FromArgb(255, 159, 0);
                    lblStatus.ForeColor = Color.White;
                    break;
                case EM_ParkingStatus.HOAN_THANH:
                    lblStatus.BackColor = Color.DarkGreen;
                    lblStatus.ForeColor = Color.White;
                    break;
            }
        }

        public void updateInfor(int STT, string ServiceCode, string ChinaPlateNum, string VietNamPlateNum, string Position, string Group, EM_ParkingStatus status)
        {
            DisplayData(STT, ServiceCode, ChinaPlateNum, VietNamPlateNum, Position, Group, status);
        }

    }
}
