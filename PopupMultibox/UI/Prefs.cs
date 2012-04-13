using System;
using System.Windows.Forms;
using System.IO;
using Multibox.Core.helpers;

namespace Multibox.Core.UI
{
// ReSharper disable InconsistentNaming
    public partial class Prefs : Form
    {
        public Prefs()
        {
            InitializeComponent();
            SearchList.Load();
            dataView.RowCount = SearchList.Count + 1;
        }

        private int currentRow = -1;
        private SearchItem curEdit;

        private void dataView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                if (e.RowIndex == dataView.Rows.Count - 1)
                    return;
                SearchItem tmp;
                tmp = e.RowIndex == currentRow ? curEdit : SearchList.Get(e.RowIndex);
                switch (e.ColumnIndex)
                {
                    case 0:
                        e.Value = tmp.Name;
                        break;
                    case 1:
                        e.Value = tmp.Keyword;
                        break;
                    case 2:
                        e.Value = tmp.SearchPath;
                        break;
                }
            }
            catch { }
        }

        private void dataView_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                SearchItem tmp;
                if (e.RowIndex < SearchList.Count)
                {
                    tmp = SearchList.Get(e.RowIndex);
                    currentRow = e.RowIndex;
                    if (curEdit == null)
                        curEdit = new SearchItem(tmp.Name, tmp.Keyword, tmp.SearchPath);
                    tmp = curEdit;
                    currentRow = e.RowIndex;
                }
                else
                {
                    tmp = curEdit;
                }
                string val = e.Value as string;
                switch (e.ColumnIndex)
                {
                    case 0:
                        tmp.Name = val;
                        break;
                    case 1:
                        tmp.Keyword = val;
                        break;
                    case 2:
                        tmp.SearchPath = val;
                        break;
                }
            }
            catch { }
        }

        private void dataView_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            curEdit = new SearchItem();
            currentRow = dataView.Rows.Count - 1;
        }

        private void dataView_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= SearchList.Count && e.RowIndex != dataView.Rows.Count)
                {
                    SearchList.Add(curEdit);
                    curEdit = null;
                    currentRow = -1;
                }
                else if (curEdit != null && e.RowIndex < SearchList.Count)
                {
                    SearchList.Set(e.RowIndex, curEdit);
                    curEdit = null;
                    currentRow = -1;
                }
                else if (dataView.ContainsFocus)
                {
                    curEdit = null;
                    currentRow = -1;
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
            if (currentRow == dataView.Rows.Count - 2 && currentRow == SearchList.Count)
            {
                curEdit = new SearchItem();
            }
            else
            {
                curEdit = null;
                currentRow = -1;
            }
        }

        private void dataView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (e.Row.Index < SearchList.Count)
                SearchList.RemoveAt(e.Row.Index);
            if (e.Row.Index != currentRow) return;
            currentRow = -1;
            curEdit = null;
        }

        private void Prefs_FormClosing(object sender, FormClosingEventArgs e)
        {
            SearchList.Store();
            PrefsManager.MultiboxWidth = (int)widthSpinner.Value;
            PrefsManager.ResultHeight = (int)heightSpinner.Value;
            PrefsManager.AutoCheckUpdate = updateCheck.Checked;
            PrefsManager.AutoCheckFrequency = (int)ufreqSpinner.Value;
            PrefsManager.Store();
            if (e.CloseReason == CloseReason.UserClosing)
                e.Cancel = true;
            Hide();
        }

        private void Prefs_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible) return;
            SearchList.Load();
            PrefsManager.Load();
            dataView.RowCount = SearchList.Count + 1;
            widthSpinner.Value = PrefsManager.MultiboxWidth;
            heightSpinner.Value = PrefsManager.ResultHeight;
            updateCheck.Checked = PrefsManager.AutoCheckUpdate;
            ufreqSpinner.Value = PrefsManager.AutoCheckFrequency;
        }

        private void updateCheck_CheckedChanged(object sender, EventArgs e)
        {
            ufreqSpinner.Enabled = updateCheck.Checked;
        }
    }
// ReSharper restore InconsistentNaming

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
                File.WriteAllLines(Environment.GetEnvironmentVariable("USERPROFILE") + "\\Popup Multibox\\prefs.txt", new[] { MultiboxWidth + "", ResultHeight + "", AutoCheckUpdate + "", AutoCheckFrequency + "" });
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