using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
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
            detailsLabel.BringToFront();
            hd = Environment.GetEnvironmentVariable("USERPROFILE");
            outputLabel.BringToFront();
            inputField.Width = this.Width - 200;
            inputField.Top = (100 - inputField.Height) / 2;
            inputField.Left = ((this.Width - 100) - inputField.Width) / 2;
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
        private bool dtltxtChanged = false;

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

        delegate void SetTextCallback(string text);

        private void SetOutputLabelText(string text)
        {
            if (this.outputLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetOutputLabelText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.outputLabel.Text = text;
                //UpdateImage();
            }
        }

        private void SetInputFieldText(string text)
        {
            if (this.inputField.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetInputFieldText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.inputField.Text = text;
                inputField.Select(inputField.Text.Length, 0);
                //UpdateImage();
            }
        }

        private void SetDetailsLabelText(string text)
        {
            if (this.detailsLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetDetailsLabelText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (!this.detailsLabel.Text.Equals(text))
                {
                    this.dtltxtChanged = true;
                    this.detailsLabel.Text = text;
                    //UpdateImage();
                }
            }
        }

        private void LMSelectionChanged(int resultIndex)
        {
            FunctionManager.SelectionChanged(this);
            //UpdateImage();
        }

        private void MainClass_SizeChanged(object sender, EventArgs e)
        {
            inputField.Width = this.Width - 200;
            inputField.Top = (100 - inputField.Height) / 2;
            inputField.Left = ((this.Width - 100) - inputField.Width) / 2;
            detailsLabel.Width = (this.Width - 200) / 2;
            detailsLabel.MaximumSize = new Size((this.Width - 200) / 2, 0);
            detailsLabel.Left = this.Width / 2;
        }

        private void inputField_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R && e.Control)
                Application.Restart();
            else if (e.KeyCode == Keys.Escape && e.Shift && !e.Control && !e.Alt)
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
            else if (e.KeyCode == Keys.F1)
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
            //UpdateImage();
        }

        delegate void USDel();

        public void UpdateSize()
        {
            if (this.InvokeRequired)
            {
                USDel d = new USDel(UpdateSize);
                this.Invoke(d, null);
            }
            else
            {
                if (this.lm != null && this.lm.ResultHeight > 0)
                {
                    int detailsLabelHeight = -1;
                    if (this.dtltxtChanged)
                    {
                        this.detailsLabel.AutoSize = true;
                        detailsLabelHeight = this.detailsLabel.Height;
                        this.detailsLabel.AutoSize = false;
                        detailsLabel.Width = (this.Width - 200) / 2;
                    }
                    int nh = -1;
                    if (detailsLabelHeight <= this.lm.WindowHeight - 120)
                        nh = this.lm.WindowHeight;
                    else
                        nh = detailsLabelHeight + 120;
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
            }
        }

        private void inputField_KeyDown(object sender, KeyEventArgs e)
        {
            FunctionManager.KeyDown(this, e);
            //UpdateImage();
        }

        private void MainClass_Load(object sender, EventArgs e)
        {
            Keys k = Keys.Space | Keys.Control | Keys.Alt;
            WindowsShell.RegisterHotKey(this, k);
        }

        // CF Note: The WndProc is not present in the Compact Framework (as of vers. 3.5)! please derive from the MessageWindow class in order to handle WM_HOTKEY
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WindowsShell.WM_HOTKEY && !prefs.Visible && !hlp.Visible && !vchk.Visible)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
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
            }
            else
            {
                this.BringToFront();
                this.CenterToScreen();
                this.Activate();
                inputField.Focus();
                if (this.Width != PrefsManager.MultiboxWidth)
                    this.Width = PrefsManager.MultiboxWidth;
                if (inputField.Width != this.Width - 200)
                {
                    inputField.Width = this.Width - 200;
                    inputField.Top = (100 - inputField.Height) / 2;
                    inputField.Left = ((this.Width - 100) - inputField.Width) / 2;
                }
                if (outputLabel.Width != this.Width - 200)
                    outputLabel.Width = this.Width - 200;
                lm.UpdateWidth(this.Width);
            }
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (prefs.Visible || hlp.Visible || vchk.Visible)
                return;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.CenterToScreen();
            inputField.Focus();
        }

        private void showItem_Click(object sender, EventArgs e)
        {
            if (prefs.Visible || hlp.Visible || vchk.Visible)
                return;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.CenterToScreen();
            inputField.Focus();
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
            updateTimer.Start();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            //base.OnPaintBackground(e);
            //GenerateBGImage();
            //e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            //e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            //e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //e.Graphics.InterpolationMode = InterpolationMode.Low;
            //e.Graphics.DrawImage(this.bgImage, new Rectangle(0, 0, this.Width, this.Height));
        }

        private void GenerateBGImage()
        {
            GenerateBGImage(1);
        }

        private void GenerateBGImage(double scale)
        {
            int w = (int)(this.Width * scale);
            int h = (int)(this.Height * scale);
            if (this.bgImage != null && this.bgImage.Width == w && this.bgImage.Height == h)
                return;
            this.bgImage = new Bitmap(w, h, PixelFormat.Format32bppArgb);
            GC.Collect();
            Graphics g = Graphics.FromImage(this.bgImage);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.InterpolationMode = InterpolationMode.Low;
            g.Clear(Color.FromArgb(0, Color.White));
            Brush b = new SolidBrush(Color.FromArgb(127, 230, 230, 230));
            if (h > 100.0 * scale)
            {
                g.FillRectangle(b, new Rectangle((int)(50.0 * scale), (int)(100.0 * scale), w - (int)(100.0 * scale), h - (int)(150.0 * scale)));
                g.FillRectangle(b, new Rectangle((int)(100.0 * scale), h - (int)(50.0 * scale), w - (int)(200.0 * scale), (int)(50.0 * scale)));
                //g.FillEllipse(b, new Rectangle((int)(50.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale)));
                //g.FillEllipse(b, new Rectangle(w - (int)(150.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale)));
                g.FillPie(b, (int)(50.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale), 180, -90);
                g.FillPie(b, w - (int)(150.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale), 0, 90);
                g.FillRectangle(Brushes.White, new Rectangle((int)(60.0 * scale), (int)(100.0 * scale), w - (int)(120.0 * scale), h - (int)(150.0 * scale)));
                g.FillRectangle(Brushes.White, new Rectangle((int)(100.0 * scale), h - (int)(50.0 * scale), w - (int)(200.0 * scale), (int)(40.0 * scale)));
                g.FillEllipse(Brushes.White, new Rectangle((int)(60.0 * scale), h - (int)(90.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
                g.FillEllipse(Brushes.White, new Rectangle(w - (int)(140.0 * scale), h - (int)(90.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            }
            g.FillRectangle(b, new Rectangle((int)(50.0 * scale), 0, w - (int)(100.0 * scale), (int)(100.0 * scale)));
            //g.FillEllipse(b, new Rectangle(0, 0, (int)(100.0 * scale), (int)(100.0 * scale)));
            //g.FillEllipse(b, new Rectangle(w - (int)(100.0 * scale), 0, (int)(100.0 * scale), (int)(100.0 * scale)));
            g.FillPie(b, 0, 0, (int)(100.0 * scale), (int)(100.0 * scale), 90, 180);
            g.FillPie(b, w - (int)(100.0 * scale), 0, (int)(100.0 * scale), (int)(100.0 * scale), 90, -180);
            g.FillRectangle(Brushes.White, new Rectangle((int)(50.0 * scale), (int)(10.0 * scale), w - (int)(100.0 * scale), (int)(80.0 * scale)));
            g.FillEllipse(Brushes.White, new Rectangle((int)(10.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            g.FillEllipse(Brushes.White, new Rectangle(w - (int)(90.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            g.DrawImage(Properties.Resources.popupMultiboxIcon3_small, new Rectangle(w - (int)(130.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
        }

        private delegate void UpdateImageDel();

        private void UpdateImage()
        {
            if (this.InvokeRequired)
            {
                UpdateImageDel d = new UpdateImageDel(UpdateImage);
                this.Invoke(d, null);
            }
            else
            {
                try
                {
                    GenerateBGImage();
                    Bitmap temp_bmp = new Bitmap(this.bgImage);
                    Rectangle b = new Rectangle(0, 0, this.Width, this.Height);
                    foreach (Control ctrl in this.Controls)
                    {
                        if (this.Visible && ctrl.Visible && b.Contains(ctrl.Bounds))
                            ctrl.DrawToBitmap(temp_bmp, ctrl.Bounds);
                    }
                    SetBitmap(temp_bmp);
                    this.Invalidate(new Rectangle(new Point(0, 0), this.Size), false);
                }
                catch { }
            }
        }

        private void MainClass_Resize(object sender, EventArgs e)
        {
            //GenerateBGImage();
            //SetBitmap(this.bgImage);
            //this.Invalidate(new Rectangle(new Point(0, 0), this.Size), false);
            //UpdateImage();
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

        /// <para>Changes the current bitmap.</para>
        public void SetBitmap(Bitmap bitmap)
        {
            SetBitmap(bitmap, 255);
        }


        /// <para>Changes the current bitmap with a custom opacity level.  Here is where all happens!</para>
        public void SetBitmap(Bitmap bitmap, byte opacity)
        {
            if (bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ApplicationException("The bitmap must be 32ppp with alpha-channel.");

            // The ideia of this is very simple,
            // 1. Create a compatible DC with screen;
            // 2. Select the bitmap with 32bpp with alpha-channel in the compatible DC;
            // 3. Call the UpdateLayeredWindow.

            IntPtr screenDc = Win32.GetDC(IntPtr.Zero);
            IntPtr memDc = Win32.CreateCompatibleDC(screenDc);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr oldBitmap = IntPtr.Zero;

            try
            {
                hBitmap = bitmap.GetHbitmap(Color.FromArgb(0));  // grab a GDI handle from this GDI+ bitmap
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
                    //Windows.DeleteObject(hBitmap); // The documentation says that we have to use the Windows.DeleteObject... but since there is no such method I use the normal DeleteObject from Win32 GDI and it's working fine without any resource leak.
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
                cp.ExStyle |= 0x00080000; // This form has to have the WS_EX_LAYERED extended style
                return cp;
            }
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            if (this.Visible)
                UpdateImage();
        }
    }

    public class WindowsShell
    {
        #region fields
        public static int MOD_ALT = 0x1;
        public static int MOD_CONTROL = 0x2;
        public static int MOD_SHIFT = 0x4;
        public static int MOD_WIN = 0x8;
        public static int WM_HOTKEY = 0x312;
        #endregion

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private static int keyId;
        public static void RegisterHotKey(Form f, Keys key)
        {
            int modifiers = 0;
            if ((key & Keys.Alt) == Keys.Alt)
                modifiers = modifiers | WindowsShell.MOD_ALT;
            if ((key & Keys.Control) == Keys.Control)
                modifiers = modifiers | WindowsShell.MOD_CONTROL;
            if ((key & Keys.Shift) == Keys.Shift)
                modifiers = modifiers | WindowsShell.MOD_SHIFT;
            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            keyId = f.GetHashCode(); // this should be a key unique ID, modify this if you want more than one hotkey
            RegisterHotKey((IntPtr)f.Handle, keyId, (uint)modifiers, (uint)k);
        }

        private delegate void Func();

        public static void UnregisterHotKey(Form f)
        {
            try
            {
                UnregisterHotKey(f.Handle, keyId); // modify this if you want more than one hotkey
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
    class Win32
    {
        public enum Bool
        {
            False = 0,
            True
        };


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y) { this.x = x; this.y = y; }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy) { this.cx = cx; this.cy = cy; }
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        public const Int32 ULW_COLORKEY = 0x00000001;
        public const Int32 ULW_ALPHA = 0x00000002;
        public const Int32 ULW_OPAQUE = 0x00000004;

        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;


        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern Bool DeleteObject(IntPtr hObject);
    }
}