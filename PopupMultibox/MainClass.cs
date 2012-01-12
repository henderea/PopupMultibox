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
            //Application.ApplicationExit += new EventHandler(AppClosing);
            PrefsManager.Load();
            lm = new LabelManager(this, PrefsManager.ResultHeight);
            lm.sc = new SelectionChanged(LMSelectionChanged);
            //lm2 = new LabelManager(this, PrefsManager.ResultHeight);
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
        //private LabelManager lm2 = null;
        private string hd;
        private Bitmap bgImage = null;
        //private static string installerPath = null;

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

        /*public LabelManager ActionLabelManager
        {
            get
            {
                return lm2;
            }
        }*/

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
                this.detailsLabel.Text = text;
            }
        }

        private void LMSelectionChanged(int resultIndex)
        {
            FunctionManager.SelectionChanged(this);
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
                prefs.Show();
                return;
            }
            else if (e.KeyCode == Keys.F1)
            {
                hlp.Show();
                return;
            }
            bool im = FunctionManager.KeyUp(this, e);
            if (e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter)
            {
                this.Hide();
                e.Handled = true;
            }
            if (im)
                outputLabel.Hide();
            else
                outputLabel.Show();
            UpdateSize();
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
                    this.detailsLabel.AutoSize = true;
                    int detailsLabelHeight = this.detailsLabel.Height;
                    this.detailsLabel.AutoSize = false;
                    detailsLabel.Width = (this.Width - 200) / 2;
                    if (detailsLabelHeight <= this.lm.WindowHeight - 120)
                        this.Height = this.lm.WindowHeight;
                    else
                        this.Height = detailsLabelHeight + 120;
                    this.detailsLabel.Height = this.Height - 120;
                    this.detailsLabel.Show();
                }
                else if (outputLabel.Text.Trim().Length <= 0)
                {
                    this.Height = 100;
                    this.detailsLabel.Hide();
                }
                else
                {
                    this.Height = 200;
                    this.detailsLabel.Hide();
                }
                this.CenterToScreen();
            }
        }

        private void inputField_KeyDown(object sender, KeyEventArgs e)
        {
            FunctionManager.KeyDown(this, e);
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
                //lm2.UpdateWidth(this.Width);
                //lm2.UpdatePosition(this.Width);
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
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            GenerateBGImage();
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            e.Graphics.SmoothingMode = SmoothingMode.HighSpeed;
            e.Graphics.DrawImage(this.bgImage, new Rectangle(0, 0, this.Width, this.Height));
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
            this.bgImage = new Bitmap(w, h);
            GC.Collect();
            Graphics g = Graphics.FromImage(this.bgImage);
            g.CompositingQuality = CompositingQuality.HighSpeed;
            g.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            g.SmoothingMode = SmoothingMode.HighSpeed;
            g.Clear(this.BackColor);
            Brush b = new SolidBrush(Color.FromArgb(230, 230, 230));
            if (h > 100.0 * scale)
            {
                g.FillRectangle(b, new Rectangle((int)(50.0 * scale), (int)(100.0 * scale), w - (int)(100.0 * scale), h - (int)(150.0 * scale)));
                g.FillRectangle(b, new Rectangle((int)(100.0 * scale), h - (int)(50.0 * scale), w - (int)(200.0 * scale), (int)(50.0 * scale)));
                g.FillEllipse(b, new Rectangle((int)(50.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale)));
                g.FillEllipse(b, new Rectangle(w - (int)(150.0 * scale), h - (int)(100.0 * scale), (int)(100.0 * scale), (int)(100.0 * scale)));
                g.FillRectangle(Brushes.White, new Rectangle((int)(60.0 * scale), (int)(100.0 * scale), w - (int)(120.0 * scale), h - (int)(150.0 * scale)));
                g.FillRectangle(Brushes.White, new Rectangle((int)(100.0 * scale), h - (int)(50.0 * scale), w - (int)(200.0 * scale), (int)(40.0 * scale)));
                g.FillEllipse(Brushes.White, new Rectangle((int)(60.0 * scale), h - (int)(90.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
                g.FillEllipse(Brushes.White, new Rectangle(w - (int)(140.0 * scale), h - (int)(90.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            }
            g.FillRectangle(b, new Rectangle((int)(50.0 * scale), 0, w - (int)(100.0 * scale), (int)(100.0 * scale)));
            g.FillEllipse(b, new Rectangle(0, 0, (int)(100.0 * scale), (int)(100.0 * scale)));
            g.FillEllipse(b, new Rectangle(w - (int)(100.0 * scale), 0, (int)(100.0 * scale), (int)(100.0 * scale)));
            g.FillRectangle(Brushes.White, new Rectangle((int)(50.0 * scale), (int)(10.0 * scale), w - (int)(100.0 * scale), (int)(80.0 * scale)));
            g.FillEllipse(Brushes.White, new Rectangle((int)(10.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            g.FillEllipse(Brushes.White, new Rectangle(w - (int)(90.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
            g.DrawImage(Properties.Resources.popupMultiboxIcon3_small, new Rectangle(w - (int)(130.0 * scale), (int)(10.0 * scale), (int)(80.0 * scale), (int)(80.0 * scale)));
        }

        private void MainClass_Resize(object sender, EventArgs e)
        {
            this.Invalidate(new Rectangle(new Point(0, 0), this.Size), false);
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
            //installerPath = path;
            Process.Start(path);
            Application.Exit();
        }

        private void AppClosing(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch { }
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
}