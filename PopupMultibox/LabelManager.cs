using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace PopupMultibox
{
    public class ResultItem
    {
        public string DisplayText
        {
            get;
            set;
        }

        public string FullText
        {
            get;
            set;
        }

        public string EvalText
        {
            get;
            set;
        }

        public ResultItem() : this("", "") { }

        public ResultItem(string d, string f) : this(d, f, "") { }

        public ResultItem(string d, string f, string e)
        {
            DisplayText = d;
            FullText = f;
            EvalText = e;
        }
    }
    public delegate void SelectionChanged(int resultIndex);
    public class LabelManager
    {
        private Label[] labels;
        private List<ResultItem> items;
        public SelectionChanged sc;
        private int resultIndex = -1;
        private int indexOffset = 0;
        private int MAX_NUM_ITEMS = 10;

        public int CurrentSelectionIndex
        {
            get
            {
                return resultIndex + indexOffset;
            }
        }

        public ResultItem CurrentSelection
        {
            get
            {
                try
                {
                    if (!(CurrentSelectionIndex < 0 || items == null || items.Count <= 0 || CurrentSelectionIndex >= items.Count))
                        return items[CurrentSelectionIndex];
                }
                catch { }
                return null;
            }
        }

        private int displayCount
        {
            get
            {
                return (items == null) ? 0 : ((items.Count >= MAX_NUM_ITEMS) ? MAX_NUM_ITEMS : items.Count);
            }
        }

        public int ResultHeight
        {
            get
            {
                return (displayCount * 50);
            }
        }

        public int WindowHeight
        {
            get
            {
                return ResultHeight + 100;
            }
        }

        public List<ResultItem> ResultItems
        {
            get
            {
                return items;
            }
            set
            {
                items = value;
                resultIndex = (items == null || items.Count <= 0) ? -1 : 0;
                indexOffset = 0;
                UpdateDisplay(true);
                if (sc != null)
                    sc.Invoke(CurrentSelectionIndex);
            }
        }

        public LabelManager(Form p, int m)
        {
            this.MAX_NUM_ITEMS = m;
            this.items = new List<ResultItem>(0);
            this.labels = new Label[MAX_NUM_ITEMS];
            p.SuspendLayout();
            for (int i = 0; i < MAX_NUM_ITEMS; i++)
            {
                this.labels[i] = new Label();
                this.labels[i].AutoEllipsis = true;
                this.labels[i].BackColor = System.Drawing.Color.Transparent;
                this.labels[i].Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                this.labels[i].Location = new System.Drawing.Point(100, 110 + (i * 50));
                this.labels[i].Size = new System.Drawing.Size(1050, 30);
                this.labels[i].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
                p.Controls.Add(this.labels[i]);
            }
            p.ResumeLayout();
            p.PerformLayout();
        }

        private delegate void UpdateDisplayDel(bool updateText);

        public void UpdateDisplay(bool updateText)
        {
            if (labels[0].InvokeRequired)
            {
                UpdateDisplayDel d = new UpdateDisplayDel(UpdateDisplay);
                labels[0].Invoke(d, new object[] { updateText });
            }
            else
            {
                for (int i = 0; i < MAX_NUM_ITEMS; i++)
                {
                    labels[i].BackColor = ((i == indexOffset && i < displayCount) ? Color.Gold : Color.Transparent);
                    if (updateText)
                        labels[i].Text = ((i < displayCount) ? items[resultIndex + i].DisplayText : "");
                }
            }
        }

        public bool SelectNext()
        {
            if (items == null || items.Count <= 1 || CurrentSelectionIndex >= items.Count - 1)
                return false;
            indexOffset++;
            if (indexOffset >= MAX_NUM_ITEMS)
            {
                indexOffset = MAX_NUM_ITEMS - 1;
                resultIndex++;
                UpdateDisplay(true);
            }
            else
                UpdateDisplay(false);
            if (sc != null)
                sc.Invoke(CurrentSelectionIndex);
            return true;
        }

        public bool SelectPrev()
        {
            if (items == null || items.Count <= 1 || CurrentSelectionIndex <= 0)
                return false;
            indexOffset--;
            if (indexOffset < 0)
            {
                indexOffset = 0;
                resultIndex--;
                UpdateDisplay(true);
            }
            else
                UpdateDisplay(false);
            if (sc != null)
                sc.Invoke(CurrentSelectionIndex);
            return true;
        }

        public void UpdateWidth(int windowWidth)
        {
            foreach (Label l in this.labels)
            {
                if (l.Width != (windowWidth - 200) / 2)
                    l.Width = (windowWidth - 200) / 2;
            }
        }
    }
}