using System;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace PopupMultibox
{
    public partial class MainClass : Form
    {
        public MainClass()
        {
            InitializeComponent();
            PrefsManager.Load();
            lm = new LabelManager(this, PrefsManager.ResultHeight);
            lm.sc = new SelectionChanged(LMSelectionChanged);
            hd = Environment.GetEnvironmentVariable("USERPROFILE");
            ResizeInputField();
            prefs = new Prefs();
            prefs.Hide();
            hlp = new Help();
            hlp.Hide();
            vchk = new VersionCheck();
        }

        private Prefs prefs;
        private Help hlp;
        private VersionCheck vchk;
        private LabelManager lm = null;
        private string hd;
        private Bitmap bgImage = null;
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
                this.SetInputFieldText(value == null ? "" : value);
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
                this.SetOutputLabelText(value == null ? "" : value);
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
                this.SetDetailsLabelText(value == null ? "" : value);
            }
        }

        private void SetOutputLabelText(string text)
        {
            if (this.outputLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetOutputLabelText);
                this.Invoke(d, new object[] { text });
                return;
            }
            this.outputLabel.Text = text;
            UpdateImage();
        }

        private void SetInputFieldText(string text)
        {
            if (this.inputField.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetInputFieldText);
                this.Invoke(d, new object[] { text });
                return;
            }
            this.inputField.Text = text;
            inputField.Select(inputField.Text.Length, 0);
            UpdateImage();
        }

        private void SetDetailsLabelText(string text)
        {
            if (this.detailsLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetDetailsLabelText);
                this.Invoke(d, new object[] { text });
                return;
            }
            if (this.detailsLabel.Text.Equals(text))
                return;
            this.detailsLabel.Text = text;
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
            detailsLabel.Width = (this.Width - 200) / 2;
            detailsLabel.MaximumSize = new Size((this.Width - 200) / 2, 0);
            detailsLabel.Left = this.Width / 2;
            UpdateImage();
        }

        private void ResizeInputField()
        {
            if (inputField.Width != this.Width - 200)
            {
                inputField.Width = this.Width - 200;
                inputField.Top = (100 - inputField.Height) / 2;
                inputField.Left = ((this.Width - 100) - inputField.Width) / 2;
            }
        }

        private void inputField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && e.Control)
                Application.Restart();
            if (e.KeyCode == Keys.Escape && e.Shift && !e.Control && !e.Alt)
            {
                e.Handled = true;
                this.Close();
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
                this.Hide();
            }
            if (im)
                outputLabel.Hide();
            else
                outputLabel.Show();
            UpdateSize();
        }

        public void UpdateSize()
        {
            if (this.InvokeRequired)
            {
                USDel d = new USDel(UpdateSize);
                this.Invoke(d, null);
                return;
            }
            if (this.lm != null && this.lm.ResultHeight > 0)
            {
                int detailsLabelHeight = -1;
                this.detailsLabel.AutoSize = true;
                detailsLabelHeight = this.detailsLabel.Height;
                this.detailsLabel.AutoSize = false;
                detailsLabel.Width = (this.Width - 200) / 2;
                int nh = -1;
                nh = (detailsLabelHeight <= this.lm.WindowHeight - 120) ? this.lm.WindowHeight : detailsLabelHeight + 120;
                if (this.Height != nh)
                {
                    this.Height = nh;
                    this.detailsLabel.Height = this.Height - 120;
                    this.detailsLabel.Show();
                }
            }
            else if (outputLabel.Text.Trim().Length <= 0)
            {
                if (this.Height != 100)
                    this.Height = 100;
                this.detailsLabel.Hide();
            }
            else
            {
                if (this.Height != 200)
                    this.Height = 200;
                this.detailsLabel.Hide();
            }
            this.CenterToScreen();
            UpdateImage();
        }

        private void inputField_KeyDown(object sender, KeyEventArgs e)
        {
            FunctionManager.KeyDown(this, e);
            UpdateImage();
        }

        private void MainClass_Load(object sender, EventArgs e)
        {
            Keys k = Keys.Space | Keys.Control | Keys.Alt;
            WindowsShell.RegisterHotKey(this, k);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WindowsShell.WM_HOTKEY && !prefs.Visible && !hlp.Visible && !vchk.Visible)
            {
                this.Show();
                this.BringToFront();
                this.CenterToScreen();
                inputField.Focus();
            }
        }

        private void MainClass_FormClosing(object sender, FormClosingEventArgs e)
        {
            WindowsShell.UnregisterHotKey(this);
        }

        private void MainClass_Deactivate(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void MainClass_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                inputField.Text = "";
                outputLabel.Text = "";
                this.lm.ResultItems = null;
                this.Height = 100;
                return;
            }
            this.BringToFront();
            this.CenterToScreen();
            this.Activate();
            inputField.Focus();
            if (this.Width != PrefsManager.MultiboxWidth)
                this.Width = PrefsManager.MultiboxWidth;
            ResizeInputField();
            if (outputLabel.Width != this.Width - 200)
                outputLabel.Width = this.Width - 200;
            lm.UpdateWidth(this.Width);
            UpdateImage();
        }

        private void ShowAndFocus()
        {
            this.Show();
            this.BringToFront();
            this.CenterToScreen();
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
            this.Close();
        }

        private void prefsItem_Click(object sender, EventArgs e)
        {
            prefs.Show();
        }

        private void MainClass_Shown(object sender, EventArgs e)
        {
            this.Hide();
            string cv = Application.ProductVersion;
            cv = cv.Remove(cv.LastIndexOf("."));
            if (!cv.Equals(Properties.Settings.Default.LastVersion))
            {
                hlp.LaunchPage("7");
                Properties.Settings.Default.LastVersion = cv;
                Properties.Settings.Default.Save();
            }
            //updateTimer.Start();
        }

        private void GenerateBGImage()
        {
            if (this.bgImage != null && this.bgImage.Width == this.Width && this.bgImage.Height == this.Height)
                return;
            this.bgImage = new Bitmap(this.Width, this.Height, PixelFormat.Format32bppArgb);
            GC.Collect();
            Graphics g = Graphics.FromImage(this.bgImage);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.Clear(Color.FromArgb(0, Color.White));
            Brush b = new SolidBrush(Color.FromArgb(127, 230, 230, 230));
            if (this.Height > 100)
            {
                g.FillRectangle(b, new Rectangle(50, 100, this.Width - 100, this.Height - 150));
                g.FillRectangle(b, new Rectangle(100, this.Height - 50, this.Width - 200, 50));
                g.FillPie(b, 50, this.Height - 100, 100, 100, 180, -90);
                g.FillPie(b, this.Width - 150, this.Height - 100, 100, 100, 0, 90);
                g.FillRectangle(Brushes.White, new Rectangle(60, 100, this.Width - 120, this.Height - 150));
                g.FillRectangle(Brushes.White, new Rectangle(100, this.Height - 50, this.Width - 200, 40));
                g.FillEllipse(Brushes.White, new Rectangle(60, this.Height - 90, 80, 80));
                g.FillEllipse(Brushes.White, new Rectangle(this.Width - 140, this.Height - 90, 80, 80));
            }
            g.FillRectangle(b, new Rectangle(50, 0, this.Width - 100, 100));
            g.FillPie(b, 0, 0, 100, 100, 90, 180);
            g.FillPie(b, this.Width - 100, 0, 100, 100, 90, -180);
            g.FillRectangle(Brushes.White, new Rectangle(50, 10, this.Width - 100, 80));
            g.FillEllipse(Brushes.White, new Rectangle(10, 10, 80, 80));
            g.FillEllipse(Brushes.White, new Rectangle(this.Width - 90, 10, 80, 80));
            g.DrawImage(Properties.Resources.popupMultiboxIcon3_small, new Rectangle(this.Width - 130, 10, 80, 80));
        }

        private void UpdateImage()
        {
            if (this.InvokeRequired)
            {
                UpdateImageDel d = new UpdateImageDel(UpdateImage);
                this.Invoke(d, null);
                return;
            }
            try
            {
                if(!this.Visible)
                    return;
                GenerateBGImage();
                Bitmap temp_bmp = new Bitmap(this.bgImage);
                Rectangle b = new Rectangle(0, 0, this.Width, this.Height);
                foreach (Control ctrl in this.Controls)
                {
                    if (this.Visible && ctrl.Visible && b.Contains(ctrl.Bounds))
                        ctrl.DrawToBitmap(temp_bmp, ctrl.Bounds);
                }
                if (this.inputField.SelectionLength == 0)
                {
                    int ss = this.inputField.SelectionStart;
                    bool sae = ss == this.inputField.Text.Length && ss > 0;
                    bool cae = false;
                    if(sae)
                    {
                        Point tmp = this.inputField.GetPositionFromCharIndex(ss - 1);
                        this.inputField.Text = this.inputField.Text + " ";
                        Point tmp2 = this.inputField.GetPositionFromCharIndex(ss - 1);
                        if (tmp2.X != tmp.X)
                            cae = true;
                    }
                    Point p = this.inputField.GetPositionFromCharIndex(ss);
                    if(sae)
                    {
                        this.inputField.Text = this.inputField.Text.Remove(ss);
                        this.inputField.Select(ss, 0);
                    }
                    if (cae)
                        p.X = this.inputField.Width;
                    Graphics g = Graphics.FromImage(temp_bmp);
                    g.FillRectangle(Brushes.Black, new Rectangle(this.inputField.Left + p.X - 1, p.Y + this.inputField.Top, 2, this.inputField.Height - p.Y * 2));
                }
                SetBitmap(temp_bmp);
                this.Invalidate(new Rectangle(new Point(0, 0), this.Size), false);
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
}