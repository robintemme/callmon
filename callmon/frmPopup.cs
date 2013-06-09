using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace callmon
{
    public partial class frmPopup : Form
    {

        public frmPopup()
        {
            InitializeComponent();
        }


        #region Glassed Frame

        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);
        [DllImport("dwmapi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public int Left;
            public int Right;
            public int Top;
            public int Bottom;
        }

        private void SetGlass()
        {
            if (DwmIsCompositionEnabled())
            {
                MARGINS margins = new MARGINS();
                margins.Top = -1; margins.Left = -1; margins.Bottom = -1; margins.Right = -1;
                DwmExtendFrameIntoClientArea(this.Handle, ref margins);
            }
        }

        #endregion

        public string NumberOrName { get; set; }
        public string TimeStamp { get; set; }
        public bool StillRinging { get; set; }

        private void frmPopup_Paint(object sender, PaintEventArgs e)
        {
            SetGlass();
        }

        private void frmPopup_VisibleChanged(object sender, EventArgs e)
        {
            lblNumber.Text = NumberOrName;
            lblDateAndTime.Text = TimeStamp;
        }
    }
}
