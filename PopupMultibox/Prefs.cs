using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace PopupMultibox
{
    public partial class Prefs : Form
    {
        public Prefs()
        {
            InitializeComponent();
            SearchList.Load();
            dataView.RowCount = SearchList.Count + 1;
        }

        private int currentRow = -1;
        private SearchItem curEdit = null;

        private void dataView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                if (e.RowIndex == dataView.Rows.Count - 1)
                    return;
                SearchItem tmp;
                if (e.RowIndex == currentRow)
                    tmp = curEdit;
                else
                    tmp = SearchList.Get(e.RowIndex);
                if (e.ColumnIndex == 0)
                    e.Value = tmp.Name;
                else if (e.ColumnIndex == 1)
                    e.Value = tmp.Keyword;
                else if (e.ColumnIndex == 2)
                    e.Value = tmp.SearchPath;
            }
            catch { }
        }

        private void dataView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                SearchItem tmp = null;
                if (e.RowIndex < SearchList.Count)
                {
                    tmp = SearchList.Get(e.RowIndex);
                    this.currentRow = e.RowIndex;
                    if (this.curEdit == null)
                        this.curEdit = new SearchItem(tmp.Name, tmp.Keyword, tmp.SearchPath);
                    tmp = this.curEdit;
                    this.currentRow = e.RowIndex;
                }
                else
                {
                    tmp = this.curEdit;
                }
                string val = e.Value as string;
                if (e.ColumnIndex == 0)
                    tmp.Name = val;
                else if (e.ColumnIndex == 1)
                    tmp.Keyword = val;
                else if (e.ColumnIndex == 2)
                    tmp.SearchPath = val;
            }
            catch { }
        }

        private void dataView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            this.curEdit = new SearchItem();
            this.currentRow = dataView.Rows.Count - 1;
        }

        private void dataView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= SearchList.Count && e.RowIndex != dataView.Rows.Count)
                {
                    SearchList.Add(this.curEdit);
                    this.curEdit = null;
                    this.currentRow = -1;
                }
                else if (this.curEdit != null && e.RowIndex < SearchList.Count)
                {
                    SearchList.Set(e.RowIndex, this.curEdit);
                    this.curEdit = null;
                    this.currentRow = -1;
                }
                else if (dataView.ContainsFocus)
                {
                    this.curEdit = null;
                    this.currentRow = -1;
                }
            }
            catch { }
        }

        private void dataView_RowDirtyStateNeeded(object sender, QuestionEventArgs e)
        {
            e.Response = dataView.IsCurrentCellDirty;
        }

        private void dataView_CancelRowEdit(object sender, QuestionEventArgs e)
        {
            if (this.currentRow == dataView.Rows.Count - 2 && this.currentRow == SearchList.Count)
            {
                this.curEdit = new SearchItem();
            }
            else
            {
                this.curEdit = null;
                this.currentRow = -1;
            }
        }

        private void dataView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Index < SearchList.Count)
                SearchList.RemoveAt(e.Row.Index);
            if (e.Row.Index == this.currentRow)
            {
                this.currentRow = -1;
                this.curEdit = null;
            }
        }

        private void Prefs_FormClosing(object sender, FormClosingEventArgs e)
        {
            SearchList.Store();
            PrefsManager.MultiboxWidth = (int)this.widthSpinner.Value;
            PrefsManager.ResultHeight = (int)this.heightSpinner.Value;
            PrefsManager.AutoCheckUpdate = this.updateCheck.Checked;
            PrefsManager.AutoCheckFrequency = (int)this.ufreqSpinner.Value;
            PrefsManager.Store();
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            this.Hide();
        }

        private void Prefs_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                SearchList.Load();
                PrefsManager.Load();
                dataView.RowCount = SearchList.Count + 1;
                this.widthSpinner.Value = PrefsManager.MultiboxWidth;
                this.heightSpinner.Value = PrefsManager.ResultHeight;
                this.updateCheck.Checked = PrefsManager.AutoCheckUpdate;
                this.ufreqSpinner.Value = PrefsManager.AutoCheckFrequency;
            }
        }

        private void updateCheck_CheckedChanged(object sender, EventArgs e)
        {
            ufreqSpinner.Enabled = updateCheck.Checked;
        }
    }

    public class PrefsManager
    {
        private static int multiboxWidth = 1250;
        private static int resultHeight = 10;
        private static bool autoCheckUpdate = true;
        private static int autoCheckFrequency = 1;

        public static int MultiboxWidth
        {
            get
            {
                return multiboxWidth;
            }
            set
            {
                if (value >= 500 && value <= 2000)
                    multiboxWidth = value;
            }
        }

        public static int ResultHeight
        {
            get
            {
                return resultHeight;
            }
            set
            {
                if (value >= 4 && value <= 20)
                    resultHeight = value;
            }
        }

        public static bool AutoCheckUpdate
        {
            get
            {
                return autoCheckUpdate;
            }
            set
            {
                autoCheckUpdate = value;
            }
        }

        public static int AutoCheckFrequency
        {
            get
            {
                return autoCheckFrequency;
            }
            set
            {
                if (value >= 1 && value <= 60)
                    autoCheckFrequency = value;
            }
        }

        public static void Store()
        {
            try
            {
                if (!Directory.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox"))
                    Directory.CreateDirectory(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox");
                // write the log file output lines to the file
                File.WriteAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\prefs.txt", new string[] { MultiboxWidth + "", ResultHeight + "", AutoCheckUpdate + "", AutoCheckFrequency + "" });
            }
            catch { }
        }

        public static void Load()
        {
            try
            {
                string[] text = File.ReadAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\prefs.txt");
                MultiboxWidth = int.Parse(text[0]);
                ResultHeight = int.Parse(text[1]);
                AutoCheckUpdate = bool.Parse(text[2]);
                AutoCheckFrequency = int.Parse(text[3]);
            }
            catch { }
        }
    }
}