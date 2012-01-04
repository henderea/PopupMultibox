namespace PopupMultibox
{
    partial class Prefs
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Prefs));
            this.dataView = new System.Windows.Forms.DataGridView();
            this.nameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.keyCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pathCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prefTabs = new System.Windows.Forms.TabControl();
            this.searchTab = new System.Windows.Forms.TabPage();
            this.updateTab = new System.Windows.Forms.TabPage();
            this.ufreqSpinner = new System.Windows.Forms.NumericUpDown();
            this.ufreqLabel = new System.Windows.Forms.Label();
            this.updateCheck = new System.Windows.Forms.CheckBox();
            this.otherTab = new System.Windows.Forms.TabPage();
            this.heightSpinner = new System.Windows.Forms.NumericUpDown();
            this.heightLabel = new System.Windows.Forms.Label();
            this.widthSpinner = new System.Windows.Forms.NumericUpDown();
            this.widthLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataView)).BeginInit();
            this.prefTabs.SuspendLayout();
            this.searchTab.SuspendLayout();
            this.updateTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ufreqSpinner)).BeginInit();
            this.otherTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heightSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // dataView
            // 
            this.dataView.AllowUserToResizeColumns = false;
            this.dataView.AllowUserToResizeRows = false;
            this.dataView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameCol,
            this.keyCol,
            this.pathCol});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataView.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataView.Location = new System.Drawing.Point(3, 3);
            this.dataView.MultiSelect = false;
            this.dataView.Name = "dataView";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataView.Size = new System.Drawing.Size(570, 530);
            this.dataView.TabIndex = 0;
            this.dataView.VirtualMode = true;
            this.dataView.CancelRowEdit += new System.Windows.Forms.QuestionEventHandler(this.dataView_CancelRowEdit);
            this.dataView.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataView_CellValueNeeded);
            this.dataView.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataView_CellValuePushed);
            this.dataView.NewRowNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataView_NewRowNeeded);
            this.dataView.RowDirtyStateNeeded += new System.Windows.Forms.QuestionEventHandler(this.dataView_RowDirtyStateNeeded);
            this.dataView.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataView_RowValidated);
            this.dataView.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.dataView_UserDeletingRow);
            // 
            // nameCol
            // 
            this.nameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.nameCol.Frozen = true;
            this.nameCol.HeaderText = "Search Name";
            this.nameCol.Name = "nameCol";
            this.nameCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.nameCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.nameCol.Width = 78;
            // 
            // keyCol
            // 
            this.keyCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.keyCol.HeaderText = "Keyword";
            this.keyCol.Name = "keyCol";
            this.keyCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.keyCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.keyCol.Width = 54;
            // 
            // pathCol
            // 
            this.pathCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.pathCol.HeaderText = "Search URL";
            this.pathCol.Name = "pathCol";
            this.pathCol.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.pathCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.pathCol.Width = 72;
            // 
            // prefTabs
            // 
            this.prefTabs.Controls.Add(this.searchTab);
            this.prefTabs.Controls.Add(this.updateTab);
            this.prefTabs.Controls.Add(this.otherTab);
            this.prefTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prefTabs.Location = new System.Drawing.Point(0, 0);
            this.prefTabs.Name = "prefTabs";
            this.prefTabs.SelectedIndex = 0;
            this.prefTabs.Size = new System.Drawing.Size(584, 562);
            this.prefTabs.TabIndex = 1;
            // 
            // searchTab
            // 
            this.searchTab.Controls.Add(this.dataView);
            this.searchTab.Location = new System.Drawing.Point(4, 22);
            this.searchTab.Name = "searchTab";
            this.searchTab.Padding = new System.Windows.Forms.Padding(3);
            this.searchTab.Size = new System.Drawing.Size(576, 536);
            this.searchTab.TabIndex = 0;
            this.searchTab.Text = "Web Searches";
            this.searchTab.UseVisualStyleBackColor = true;
            // 
            // updateTab
            // 
            this.updateTab.Controls.Add(this.ufreqSpinner);
            this.updateTab.Controls.Add(this.ufreqLabel);
            this.updateTab.Controls.Add(this.updateCheck);
            this.updateTab.Location = new System.Drawing.Point(4, 22);
            this.updateTab.Name = "updateTab";
            this.updateTab.Padding = new System.Windows.Forms.Padding(3);
            this.updateTab.Size = new System.Drawing.Size(576, 536);
            this.updateTab.TabIndex = 2;
            this.updateTab.Text = "Updates";
            this.updateTab.UseVisualStyleBackColor = true;
            // 
            // ufreqSpinner
            // 
            this.ufreqSpinner.Location = new System.Drawing.Point(250, 32);
            this.ufreqSpinner.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.ufreqSpinner.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.ufreqSpinner.Name = "ufreqSpinner";
            this.ufreqSpinner.Size = new System.Drawing.Size(75, 20);
            this.ufreqSpinner.TabIndex = 2;
            this.ufreqSpinner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ufreqSpinner.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // ufreqLabel
            // 
            this.ufreqLabel.AutoSize = true;
            this.ufreqLabel.Location = new System.Drawing.Point(50, 30);
            this.ufreqLabel.Name = "ufreqLabel";
            this.ufreqLabel.Padding = new System.Windows.Forms.Padding(5);
            this.ufreqLabel.Size = new System.Drawing.Size(192, 23);
            this.ufreqLabel.TabIndex = 1;
            this.ufreqLabel.Text = "Days between update checks (1-60):";
            // 
            // updateCheck
            // 
            this.updateCheck.AutoSize = true;
            this.updateCheck.Checked = true;
            this.updateCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.updateCheck.Location = new System.Drawing.Point(0, 0);
            this.updateCheck.Name = "updateCheck";
            this.updateCheck.Padding = new System.Windows.Forms.Padding(5);
            this.updateCheck.Size = new System.Drawing.Size(187, 27);
            this.updateCheck.TabIndex = 0;
            this.updateCheck.Text = "Automatically check for updates";
            this.updateCheck.UseVisualStyleBackColor = true;
            this.updateCheck.CheckedChanged += new System.EventHandler(this.updateCheck_CheckedChanged);
            // 
            // otherTab
            // 
            this.otherTab.Controls.Add(this.heightSpinner);
            this.otherTab.Controls.Add(this.heightLabel);
            this.otherTab.Controls.Add(this.widthSpinner);
            this.otherTab.Controls.Add(this.widthLabel);
            this.otherTab.Location = new System.Drawing.Point(4, 22);
            this.otherTab.Name = "otherTab";
            this.otherTab.Padding = new System.Windows.Forms.Padding(3);
            this.otherTab.Size = new System.Drawing.Size(576, 536);
            this.otherTab.TabIndex = 1;
            this.otherTab.Text = "Other Options";
            this.otherTab.UseVisualStyleBackColor = true;
            // 
            // heightSpinner
            // 
            this.heightSpinner.Location = new System.Drawing.Point(275, 27);
            this.heightSpinner.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.heightSpinner.Minimum = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.heightSpinner.Name = "heightSpinner";
            this.heightSpinner.Size = new System.Drawing.Size(75, 20);
            this.heightSpinner.TabIndex = 3;
            this.heightSpinner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.heightSpinner.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // heightLabel
            // 
            this.heightLabel.AutoSize = true;
            this.heightLabel.Location = new System.Drawing.Point(0, 25);
            this.heightLabel.Margin = new System.Windows.Forms.Padding(0);
            this.heightLabel.Name = "heightLabel";
            this.heightLabel.Padding = new System.Windows.Forms.Padding(5);
            this.heightLabel.Size = new System.Drawing.Size(262, 23);
            this.heightLabel.TabIndex = 2;
            this.heightLabel.Text = "Result Height (2-10 items) (takes effect after restart):";
            // 
            // widthSpinner
            // 
            this.widthSpinner.Location = new System.Drawing.Point(175, 2);
            this.widthSpinner.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.widthSpinner.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.widthSpinner.Name = "widthSpinner";
            this.widthSpinner.Size = new System.Drawing.Size(100, 20);
            this.widthSpinner.TabIndex = 1;
            this.widthSpinner.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.widthSpinner.Value = new decimal(new int[] {
            1250,
            0,
            0,
            0});
            // 
            // widthLabel
            // 
            this.widthLabel.AutoSize = true;
            this.widthLabel.Location = new System.Drawing.Point(0, 0);
            this.widthLabel.Margin = new System.Windows.Forms.Padding(0);
            this.widthLabel.Name = "widthLabel";
            this.widthLabel.Padding = new System.Windows.Forms.Padding(5);
            this.widthLabel.Size = new System.Drawing.Size(169, 23);
            this.widthLabel.TabIndex = 0;
            this.widthLabel.Text = "Width of multibox (500-2000 px):";
            // 
            // Prefs
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 562);
            this.Controls.Add(this.prefTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Prefs";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Preferences";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Prefs_FormClosing);
            this.VisibleChanged += new System.EventHandler(this.Prefs_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.dataView)).EndInit();
            this.prefTabs.ResumeLayout(false);
            this.searchTab.ResumeLayout(false);
            this.updateTab.ResumeLayout(false);
            this.updateTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ufreqSpinner)).EndInit();
            this.otherTab.ResumeLayout(false);
            this.otherTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.heightSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.widthSpinner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataView;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn pathCol;
        private System.Windows.Forms.TabControl prefTabs;
        private System.Windows.Forms.TabPage searchTab;
        private System.Windows.Forms.TabPage otherTab;
        private System.Windows.Forms.NumericUpDown widthSpinner;
        private System.Windows.Forms.Label widthLabel;
        private System.Windows.Forms.NumericUpDown heightSpinner;
        private System.Windows.Forms.Label heightLabel;
        private System.Windows.Forms.TabPage updateTab;
        private System.Windows.Forms.CheckBox updateCheck;
        private System.Windows.Forms.Label ufreqLabel;
        private System.Windows.Forms.NumericUpDown ufreqSpinner;
    }
}