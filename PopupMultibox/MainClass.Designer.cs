namespace PopupMultibox
{
    partial class MainClass
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainClass));
            this.inputField = new System.Windows.Forms.TextBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.trayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showItem = new System.Windows.Forms.ToolStripMenuItem();
            this.prefsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restartItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.detailsLabel = new System.Windows.Forms.Label();
            this.trayMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputField
            // 
            this.inputField.BackColor = System.Drawing.Color.White;
            this.inputField.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.inputField.Font = new System.Drawing.Font("Microsoft Sans Serif", 50.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputField.ForeColor = System.Drawing.Color.RoyalBlue;
            this.inputField.Location = new System.Drawing.Point(44, 12);
            this.inputField.Name = "inputField";
            this.inputField.Size = new System.Drawing.Size(1050, 76);
            this.inputField.TabIndex = 0;
            this.inputField.WordWrap = false;
            this.inputField.KeyDown += new System.Windows.Forms.KeyEventHandler(this.inputField_KeyDown);
            this.inputField.KeyUp += new System.Windows.Forms.KeyEventHandler(this.inputField_KeyUp);
            // 
            // outputLabel
            // 
            this.outputLabel.AutoEllipsis = true;
            this.outputLabel.BackColor = System.Drawing.Color.Transparent;
            this.outputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputLabel.Location = new System.Drawing.Point(100, 110);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(1050, 80);
            this.outputLabel.TabIndex = 1;
            this.outputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trayIcon
            // 
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("trayIcon.Icon")));
            this.trayIcon.Text = "Popup Multibox";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.trayIcon_MouseDoubleClick);
            // 
            // trayMenu
            // 
            this.trayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showItem,
            this.prefsItem,
            this.helpItem,
            this.restartItem,
            this.exitItem});
            this.trayMenu.Name = "trayMenu";
            this.trayMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.trayMenu.ShowImageMargin = false;
            this.trayMenu.Size = new System.Drawing.Size(186, 114);
            // 
            // showItem
            // 
            this.showItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.showItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.showItem.Name = "showItem";
            this.showItem.ShortcutKeyDisplayString = "CTRL+ALT+Space";
            this.showItem.Size = new System.Drawing.Size(185, 22);
            this.showItem.Text = "Show";
            this.showItem.Click += new System.EventHandler(this.showItem_Click);
            // 
            // prefsItem
            // 
            this.prefsItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.prefsItem.Name = "prefsItem";
            this.prefsItem.ShortcutKeyDisplayString = "CTRL+P";
            this.prefsItem.Size = new System.Drawing.Size(185, 22);
            this.prefsItem.Text = "Preferences";
            this.prefsItem.Click += new System.EventHandler(this.prefsItem_Click);
            // 
            // helpItem
            // 
            this.helpItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.helpItem.Name = "helpItem";
            this.helpItem.ShortcutKeyDisplayString = "F1";
            this.helpItem.Size = new System.Drawing.Size(185, 22);
            this.helpItem.Text = "Help";
            this.helpItem.Click += new System.EventHandler(this.helpItem_Click);
            // 
            // restartItem
            // 
            this.restartItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.restartItem.Name = "restartItem";
            this.restartItem.ShortcutKeyDisplayString = "CTRL+R";
            this.restartItem.Size = new System.Drawing.Size(185, 22);
            this.restartItem.Text = "Restart";
            this.restartItem.Click += new System.EventHandler(this.restartItem_Click);
            // 
            // exitItem
            // 
            this.exitItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.exitItem.Name = "exitItem";
            this.exitItem.ShortcutKeyDisplayString = "SHIFT+ESC";
            this.exitItem.Size = new System.Drawing.Size(185, 22);
            this.exitItem.Text = "Exit";
            this.exitItem.Click += new System.EventHandler(this.exitItem_Click);
            // 
            // detailsLabel
            // 
            this.detailsLabel.AutoEllipsis = true;
            this.detailsLabel.BackColor = System.Drawing.Color.Transparent;
            this.detailsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detailsLabel.Location = new System.Drawing.Point(625, 110);
            this.detailsLabel.Name = "detailsLabel";
            this.detailsLabel.Size = new System.Drawing.Size(525, 30);
            this.detailsLabel.TabIndex = 2;
            this.detailsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.ClientSize = new System.Drawing.Size(1250, 100);
            this.Controls.Add(this.detailsLabel);
            this.Controls.Add(this.outputLabel);
            this.Controls.Add(this.inputField);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.RoyalBlue;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainClass";
            this.Opacity = 0.9D;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Popup Multibox";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.Deactivate += new System.EventHandler(this.MainClass_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainClass_FormClosing);
            this.Load += new System.EventHandler(this.MainClass_Load);
            this.Shown += new System.EventHandler(this.MainClass_Shown);
            this.SizeChanged += new System.EventHandler(this.MainClass_SizeChanged);
            this.VisibleChanged += new System.EventHandler(this.MainClass_VisibleChanged);
            this.Resize += new System.EventHandler(this.MainClass_Resize);
            this.trayMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputField;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.NotifyIcon trayIcon;
        private System.Windows.Forms.ContextMenuStrip trayMenu;
        private System.Windows.Forms.ToolStripMenuItem showItem;
        private System.Windows.Forms.ToolStripMenuItem exitItem;
        private System.Windows.Forms.ToolStripMenuItem prefsItem;
        private System.Windows.Forms.ToolStripMenuItem helpItem;
        private System.Windows.Forms.ToolStripMenuItem restartItem;
        private System.Windows.Forms.Label detailsLabel;
    }
}

