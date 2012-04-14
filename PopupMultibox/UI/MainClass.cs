using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Multibox.Core.Functions;
using Multibox.Core.helpers;

namespace Multibox.Core.UI
{
// ReSharper disable InconsistentNaming
    public partial class MainClass : Form
    {
        public MainClass()
        {
            InitializeComponent();
            FunctionManager.Setup();
            PrefsManager.Load();
            lm = new LabelManager(this, PrefsManager.ResultHeight);
            lm.Sc = LMSelectionChanged;
            hd = Environment.GetEnvironmentVariable("USERPROFILE");
            ResizeInputField();
            prefs = new Prefs();
            prefs.Hide();
            hlp = new Help();
            hlp.Hide();
            vchk = new VersionCheck();
        }

        private readonly Prefs prefs;
        private readonly Help hlp;
        private readonly VersionCheck vchk;
        private readonly LabelManager lm;
        private readonly string hd;
        private Bitmap bgImage;
        delegate void SetTextCallback(string text);
        delegate void USDel();
        private delegate void UpdateImageDel();

        public Prefs PreferencesDialog
        {
            get
            {
                return prefs;
            }
        }

        public Help HelpDialog
        {
            get
            {
                return hlp;
            }
        }

        public LabelManager LabelManager
        {
            get
            {
                return lm;
            }
        }

        public VersionCheck VChk
        {
            get
            {
                return vchk;
            }
        }

        public string HomeDirectory
        {
            get
            {
                return hd;
            }
        }

        public string InputFieldText
        {
            get
            {
                return inputField.Text;
            }
            set
            {
                SetInputFieldText(value ?? "");
            }
        }

        public string OutputLabelText
        {
            get
            {
                return outputLabel.Text;
            }
            set
            {
                SetOutputLabelText(value ?? "");
            }
        }

        public string DetailsLabelText
        {
            get
            {
                return detailsLabel.Text;
            }
            set
            {
                SetDetailsLabelText(value ?? "");
            }
        }

        private void SetOutputLabelText(string text)
        {
            if (outputLabel.InvokeRequired)
            {
                SetTextCallback d = SetOutputLabelText;
                Invoke(d, new object[] { text });
                return;
            }
            outputLabel.Text = text;
            UpdateImage();
        }

        private void SetInputFieldText(string text)
        {
            if (inputField.InvokeRequired)
            {
                SetTextCallback d = SetInputFieldText;
                Invoke(d, new object[] { text });
                return;
            }
            inputField.Text = text;
            inputField.Select(inputField.Text.Length, 0);
            UpdateImage();
        }

        private void SetDetailsLabelText(string text)
        {
            if (detailsLabel.InvokeRequired)
            {
                SetTextCallback d = SetDetailsLabelText;
                Invoke(d, new object[] { text });
                return;
            }
            if (detailsLabel.Text.Equals(text))
                return;
            detailsLabel.Text = text;
            UpdateImage();
        }

        private void LMSelectionChanged(int resultIndex)
        {
            FunctionManager.SelectionChanged(this);
            UpdateImage();
        }

        private void MainClass_SizeChanged(object sender, EventArgs e)
        {
            ResizeInputField();
            detailsLabel.Width = (Width - 200) / 2;
            detailsLabel.MaximumSize = new Size((Width - 200) / 2, 0);
            detailsLabel.Left = Width / 2;
            UpdateImage();
        }

        private void ResizeInputField()
        {
            if (inputField.Width != Width - 200)
            {
                inputField.Width = Width - 200;
                inputField.Top = (100 - inputField.Height) / 2;
                inputField.Left = ((Width - 100) - inputField.Width) / 2;
            }
        }

        private void inputField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && e.Control)
                Application.Restart();
            if (e.KeyCode == Keys.Escape && e.Shift && !e.Control && !e.Alt)
            {
                e.Handled = true;
                Close();
                return;
            }
            if (e.KeyCode == Keys.P && e.Control)
            {
                e.Handled = true;
                prefs.Show();
                return;
            }
            if (e.KeyCode == Keys.F1)
            {
                e.Handled = true;
                hlp.Show();
                return;
            }
            bool im = FunctionManager.KeyUp(this, e);
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                Hide();
            }
            if (im)
                outputLabel.Hide();
            else
                outputLabel.Show();
            UpdateSize();
        }

        public void UpdateSize()
        {
            if (InvokeRequired)
            {
                USDel d = UpdateSize;
                Invoke(d, null);
                return;
            }
            if (lm != null && lm.ResultHeight > 0)
            {
                int detailsLabelHeight;
                detailsLabel.AutoSize = true;
                detailsLabelHeight = detailsLabel.Height;
                detailsLabel.AutoSize = false;
                detailsLabel.Width = (Width - 200) / 2;
                int nh;
                nh = (detailsLabelHeight <= lm.WindowHeight - 120) ? lm.WindowHeight : detailsLabelHeight + 120;
                if (Height != nh)
                {
                    Height = nh;
                    detailsLabel.Height = Height - 120;
                    detailsLabel.Show();
                }
            }
            else if (outputLabel.Text.Trim().Length <= 0)
            {
                Height = 100;
                detailsLabel.Hide();
            }
            else
            {
                Height = 200;
                detailsLabel.Hide();
            }
            CenterToScreen();
            UpdateImage();
        }

        private void inputField_KeyDown(object sender, KeyEventArgs e)
        {
            FunctionManager.KeyDown(this, e);
            UpdateImage();
        }

        private void MainClass_Load(object sender, EventArgs e)
        {
            const Keys k = Keys.Space | Keys.Control | Keys.Alt;
            WindowsShell.RegisterHotKey(this, k);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg != WindowsShell.WM_HOTKEY || prefs.Visible || hlp.Visible || vchk.Visible) return;
            Show();
            BringToFront();
            CenterToScreen();
            inputField.Focus();
        }

        private void MainClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowsShell.UnregisterHotKey(this);
        }

        private void MainClass_Deactivate(object sender, EventArgs e)
        {
            Hide();
        }

        private void MainClass_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible)
            {
                inputField.Text = "";
                outputLabel.Text = "";
                lm.ResultItems = null;
                Height = 100;
                return;
            }
            BringToFront();
            CenterToScreen();
            Activate();
            inputField.Focus();
            Width = PrefsManager.MultiboxWidth;
            ResizeInputField();
            outputLabel.Width = Width - 200;
            lm.UpdateWidth(Width);
            UpdateImage();
        }

        private void ShowAndFocus()
        {
            Show();
            BringToFront();
            CenterToScreen();
            inputField.Focus();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (prefs.Visible || hlp.Visible || vchk.Visible)
                return;
            ShowAndFocus();
        }

        private void showItem_Click(object sender, EventArgs e)
        {
            if (prefs.Visible || hlp.Visible || vchk.Visible)
                return;
            ShowAndFocus();
        }

        private void exitItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void prefsItem_Click(object sender, EventArgs e)
        {
            prefs.Show();
        }

        private void MainClass_Shown(object sender, EventArgs e)
        {
            Hide();
            string cv = Application.ProductVersion;
            cv = cv.Remove(cv.LastIndexOf("."));
            if (cv.Equals(Properties.Settings.Default.LastVersion)) return;
            vchk.checkForUpdateForce();
            hlp.LaunchPage("7");
            Properties.Settings.Default.LastVersion = cv;
            Properties.Settings.Default.Save();
            //updateTimer.Start();
        }

        private void GenerateBGImage()
        {
            if (bgImage != null && bgImage.Width == Width && bgImage.Height == Height)
                return;
            bgImage = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);
            GC.Collect();
            Graphics g = Graphics.FromImage(bgImage);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.Clear(Color.FromArgb(0, Color.White));
            Brush b = new SolidBrush(Color.FromArgb(127, 230, 230, 230));
            if (Height > 100)
            {
                g.FillRectangle(b, new Rectangle(50, 100, Width - 100, Height - 150));
                g.FillRectangle(b, new Rectangle(100, Height - 50, Width - 200, 50));
                g.FillPie(b, 50, Height - 100, 100, 100, 180, -90);
                g.FillPie(b, Width - 150, Height - 100, 100, 100, 0, 90);
                g.FillRectangle(Brushes.White, new Rectangle(60, 100, Width - 120, Height - 150));
                g.FillRectangle(Brushes.White, new Rectangle(100, Height - 50, Width - 200, 40));
                g.FillEllipse(Brushes.White, new Rectangle(60, Height - 90, 80, 80));
                g.FillEllipse(Brushes.White, new Rectangle(Width - 140, Height - 90, 80, 80));
            }
            g.FillRectangle(b, new Rectangle(50, 0, Width - 100, 100));
            g.FillPie(b, 0, 0, 100, 100, 90, 180);
            g.FillPie(b, Width - 100, 0, 100, 100, 90, -180);
            g.FillRectangle(Brushes.White, new Rectangle(50, 10, Width - 100, 80));
            g.FillEllipse(Brushes.White, new Rectangle(10, 10, 80, 80));
            g.FillEllipse(Brushes.White, new Rectangle(Width - 90, 10, 80, 80));
            g.DrawImage(Properties.Resources.popupMultiboxIcon3_small, new Rectangle(Width - 130, 10, 80, 80));
        }

        private void UpdateImage()
        {
            if (InvokeRequired)
            {
                UpdateImageDel d = UpdateImage;
                Invoke(d, null);
                return;
            }
            try
            {
                if(!Visible)
                    return;
                GenerateBGImage();
                Bitmap temp_bmp = new Bitmap(bgImage);
                Rectangle b = new Rectangle(0, 0, Width, Height);
                foreach (Control ctrl in Controls)
                {
                    if (Visible && ctrl.Visible && b.Contains(ctrl.Bounds))
                        ctrl.DrawToBitmap(temp_bmp, ctrl.Bounds);
                }
                if (inputField.SelectionLength == 0)
                {
                    int ss = inputField.SelectionStart;
                    bool sae = ss == inputField.Text.Length && ss > 0;
                    bool cae = false;
                    if(sae)
                    {
                        Point tmp = inputField.GetPositionFromCharIndex(ss - 1);
                        inputField.Text = inputField.Text + " ";
                        Point tmp2 = inputField.GetPositionFromCharIndex(ss - 1);
                        if (tmp2.X != tmp.X)
                            cae = true;
                    }
                    Point p = inputField.GetPositionFromCharIndex(ss);
                    if(sae)
                    {
                        inputField.Text = inputField.Text.Remove(ss);
                        inputField.Select(ss, 0);
                    }
                    if (cae)
                        p.X = inputField.Width;
                    Graphics g = Graphics.FromImage(temp_bmp);
                    g.FillRectangle(Brushes.Black, new Rectangle(inputField.Left + p.X - 1, p.Y + inputField.Top, 2, inputField.Height - p.Y * 2));
                }
                SetBitmap(temp_bmp);
                Invalidate(new Rectangle(new Point(0, 0), Size), false);
            }
            catch { }
        }

        private void helpItem_Click(object sender, EventArgs e)
        {
            hlp.Show();
        }

        private void restartItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        public static void CloseAndInstall(string path)
        {
            Process.Start(path);
            Application.Exit();
        }

        public void SetBitmap(Bitmap bitmap)
        {
            SetBitmap(bitmap, 255);
        }

        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");
            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;
            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));
                oldBitmap = Win32.SelectObject(memDc, hBitmap);
                Win32.Size size = new Win32.Size(bitmap.Width, bitmap.Height);
                Win32.Point pointSource = new Win32.Point(0, 0);
                Win32.Point topPos = new Win32.Point(Left, Top);
                Win32.BLENDFUNCTION blend = new Win32.BLENDFUNCTION();
                blend.BlendOp = Win32.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = Win32.AC_SRC_ALPHA;
                Win32.UpdateLayeredWindow(Handle, screenDc, ref topPos, ref size, memDc, ref pointSource, 0, ref blend, Win32.ULW_ALPHA);
            }
            finally
            {
                Win32.ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    Win32.SelectObject(memDc, oldBitmap);
                    Win32.DeleteObject(hBitmap);
                }
                Win32.DeleteDC(memDc);
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00080000;
                return cp;
            }
        }
    }
// ReSharper restore InconsistentNaming
}