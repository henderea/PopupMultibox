namespace Multibox.PluginUpdater
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainClass));
            this.updateList = new System.Windows.Forms.DataGridView();
            this.updateCheckCol = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.pluginNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.curVersionCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.newVersionCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.refreshButton = new System.Windows.Forms.Button();
            this.noteLabel = new System.Windows.Forms.Label();
            this.checkButton = new System.Windows.Forms.Button();
            this.uncheckButton = new System.Windows.Forms.Button();
            this.installButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.closeButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.updateList)).BeginInit();
            this.SuspendLayout();
            // 
            // updateList
            // 
            this.updateList.AllowUserToAddRows = false;
            this.updateList.AllowUserToDeleteRows = false;
            this.updateList.AllowUserToResizeColumns = false;
            this.updateList.AllowUserToResizeRows = false;
            this.updateList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.updateList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.updateList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.updateList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.updateCheckCol,
            this.pluginNameCol,
            this.curVersionCol,
            this.newVersionCol});
            this.updateList.Location = new System.Drawing.Point(0, 0);
            this.updateList.MultiSelect = false;
            this.updateList.Name = "updateList";
            this.updateList.Size = new System.Drawing.Size(750, 500);
            this.updateList.TabIndex = 0;
            // 
            // updateCheckCol
            // 
            this.updateCheckCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.updateCheckCol.HeaderText = "Include in Update";
            this.updateCheckCol.Name = "updateCheckCol";
            this.updateCheckCol.Width = 97;
            // 
            // pluginNameCol
            // 
            this.pluginNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pluginNameCol.HeaderText = "Plugin";
            this.pluginNameCol.Name = "pluginNameCol";
            this.pluginNameCol.ReadOnly = true;
            this.pluginNameCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.pluginNameCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pluginNameCol.Width = 42;
            // 
            // curVersionCol
            // 
            this.curVersionCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.curVersionCol.HeaderText = "Your Version";
            this.curVersionCol.Name = "curVersionCol";
            this.curVersionCol.ReadOnly = true;
            this.curVersionCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.curVersionCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.curVersionCol.Width = 73;
            // 
            // newVersionCol
            // 
            this.newVersionCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.newVersionCol.HeaderText = "New Version";
            this.newVersionCol.Name = "newVersionCol";
            this.newVersionCol.ReadOnly = true;
            this.newVersionCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.newVersionCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.newVersionCol.Width = 73;
            // 
            // refreshButton
            // 
            this.refreshButton.Location = new System.Drawing.Point(0, 500);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(100, 25);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh List";
            this.refreshButton.UseVisualStyleBackColor = true;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // noteLabel
            // 
            this.noteLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noteLabel.Location = new System.Drawing.Point(0, 525);
            this.noteLabel.Margin = new System.Windows.Forms.Padding(0);
            this.noteLabel.Name = "noteLabel";
            this.noteLabel.Padding = new System.Windows.Forms.Padding(5);
            this.noteLabel.Size = new System.Drawing.Size(750, 50);
            this.noteLabel.TabIndex = 2;
            this.noteLabel.Text = "NOTE: Do not try to update plugins while the main Multibox program is still runni" +
    "ng.";
            this.noteLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkButton
            // 
            this.checkButton.Location = new System.Drawing.Point(100, 500);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(100, 25);
            this.checkButton.TabIndex = 3;
            this.checkButton.Text = "Check All";
            this.checkButton.UseVisualStyleBackColor = true;
            this.checkButton.Click += new System.EventHandler(this.checkButton_Click);
            // 
            // uncheckButton
            // 
            this.uncheckButton.Location = new System.Drawing.Point(200, 500);
            this.uncheckButton.Name = "uncheckButton";
            this.uncheckButton.Size = new System.Drawing.Size(100, 25);
            this.uncheckButton.TabIndex = 4;
            this.uncheckButton.Text = "Uncheck All";
            this.uncheckButton.UseVisualStyleBackColor = true;
            this.uncheckButton.Click += new System.EventHandler(this.uncheckButton_Click);
            // 
            // installButton
            // 
            this.installButton.Location = new System.Drawing.Point(300, 500);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(100, 25);
            this.installButton.TabIndex = 5;
            this.installButton.Text = "Install Plugins";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.installButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(400, 500);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 25);
            this.progressBar.TabIndex = 6;
            // 
            // closeButton
            // 
            this.closeButton.Location = new System.Drawing.Point(600, 500);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(150, 25);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close amd Start Multibox";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // MainClass
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(909, 615);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.uncheckButton);
            this.Controls.Add(this.checkButton);
            this.Controls.Add(this.noteLabel);
            this.Controls.Add(this.refreshButton);
            this.Controls.Add(this.updateList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainClass";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Plugin Updater";
            ((System.ComponentModel.ISupportInitialize)(this.updateList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView updateList;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Label noteLabel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn updateCheckCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pluginNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn curVersionCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn newVersionCol;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Button uncheckButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button closeButton;
    }
}

