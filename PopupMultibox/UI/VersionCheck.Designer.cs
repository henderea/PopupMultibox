namespace PopupMultibox.UI
{
    partial class VersionCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VersionCheck));
            this.displayLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.okButton = new System.Windows.Forms.Button();
            this.checkTimer = new System.Windows.Forms.Timer(this.components);
            this.downloadButton = new System.Windows.Forms.Button();
            this.fileChooserS = new System.Windows.Forms.SaveFileDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.installButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // displayLabel
            // 
            this.displayLabel.Location = new System.Drawing.Point(0, 0);
            this.displayLabel.Margin = new System.Windows.Forms.Padding(0);
            this.displayLabel.Name = "displayLabel";
            this.displayLabel.Padding = new System.Windows.Forms.Padding(5);
            this.displayLabel.Size = new System.Drawing.Size(300, 75);
            this.displayLabel.TabIndex = 0;
            this.displayLabel.Text = resources.GetString("displayLabel.Text");
            this.displayLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // versionLabel
            // 
            this.versionLabel.Location = new System.Drawing.Point(0, 75);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(300, 75);
            this.versionLabel.TabIndex = 1;
            this.versionLabel.Text = "Current version: 1.1.1\r\n\r\nNew version: 1.1.2";
            this.versionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // linkLabel
            // 
            this.linkLabel.Location = new System.Drawing.Point(0, 150);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(300, 25);
            this.linkLabel.TabIndex = 2;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "http://multibox.everydayprogramminggenius.com/download";
            this.linkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.linkLabel.VisitedLinkColor = System.Drawing.Color.Blue;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(25, 200);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(50, 25);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // checkTimer
            // 
            this.checkTimer.Interval = 3600000;
            this.checkTimer.Tick += new System.EventHandler(this.checkTimer_Tick);
            // 
            // downloadButton
            // 
            this.downloadButton.Location = new System.Drawing.Point(125, 200);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(150, 25);
            this.downloadButton.TabIndex = 4;
            this.downloadButton.Text = "Download and Install";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // fileChooserS
            // 
            this.fileChooserS.FileName = "MultiboxInstaller.msi";
            this.fileChooserS.Filter = "MSI Installer|*.msi";
            this.fileChooserS.FileOk += new System.ComponentModel.CancelEventHandler(this.fileChooserS_FileOk);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(0, 200);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(225, 25);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            // 
            // installButton
            // 
            this.installButton.Enabled = false;
            this.installButton.Location = new System.Drawing.Point(225, 200);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(75, 25);
            this.installButton.TabIndex = 6;
            this.installButton.Text = "Install";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Visible = false;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // VersionCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(383, 305);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.linkLabel);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.displayLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "VersionCheck";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "New Multibox version available";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VersionCheck_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label displayLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.LinkLabel linkLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Timer checkTimer;
        private System.Windows.Forms.Button downloadButton;
        private System.Windows.Forms.SaveFileDialog fileChooserS;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button installButton;
    }
}