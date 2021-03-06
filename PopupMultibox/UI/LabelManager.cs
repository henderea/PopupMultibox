﻿using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace Multibox.Core.UI
{
    public class ResultItem
    {
        protected bool Equals(ResultItem other)
        {
            return string.Equals(DisplayText, other.DisplayText) && string.Equals(FullText, other.FullText) && string.Equals(EvalText, other.EvalText);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (DisplayText != null ? DisplayText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FullText != null ? FullText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (EvalText != null ? EvalText.GetHashCode() : 0);
                return hashCode;
            }
        }

        public string DisplayText { get; set; }

        public string FullText { get; set; }

        public string EvalText { get; set; }

        public ResultItem() : this("", "") { }

        public ResultItem(string d, string f) : this(d, f, "") { }

        public ResultItem(string d, string f, string e)
        {
            DisplayText = d;
            FullText = f;
            EvalText = e;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ResultItem) obj);
        }
    }
    public delegate void SelectionChanged(int resultIndex);
    public class LabelManager : ILabelManager
    {
        private readonly Label[] labels;
        private List<ResultItem> items;
        public SelectionChanged Sc { get; set; }
        private int resultIndex = -1;
        private int indexOffset;
        private static int maxNumItems = 10;

        public int ResultIndex
        {
            get
            {
                return resultIndex;
            }
        }
        
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

        public int DisplayCount
        {
            get
            {
                return (items == null) ? 0 : ((items.Count >= maxNumItems) ? maxNumItems : items.Count);
            }
        }

        public int ResultHeight
        {
            get
            {
                return (DisplayCount * 50);
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
                if (Sc != null)
                    Sc.Invoke(CurrentSelectionIndex);
            }
        }

        public LabelManager(Form p, int m)
        {
            maxNumItems = m;
            items = new List<ResultItem>(0);
            labels = new Label[maxNumItems];
            p.SuspendLayout();
            for (int i = 0; i < maxNumItems; i++)
            {
                labels[i] = new Label();
                labels[i].AutoEllipsis = true;
                labels[i].BackColor = Color.White;
                labels[i].Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Regular, GraphicsUnit.Point, 0);
                labels[i].Location = new Point(100, 110 + (i * 50));
                labels[i].Size = new Size(1050, 30);
                labels[i].TextAlign = ContentAlignment.MiddleCenter;
                p.Controls.Add(labels[i]);
            }
            p.ResumeLayout();
            p.PerformLayout();
        }

        private delegate void UpdateDisplayDel(bool updateText);

        public void UpdateDisplay(bool updateText)
        {
            UpdateVisibility(resultIndex >= 0);
            if (labels[0].InvokeRequired)
            {
                UpdateDisplayDel d = UpdateDisplay;
                labels[0].Invoke(d, new object[] { updateText });
            }
            else
            {
                for (int i = 0; i < maxNumItems; i++)
                {
                    labels[i].BackColor = ((i == indexOffset && i < DisplayCount) ? Color.Gold : Color.White);
                    if (updateText)
                        labels[i].Text = ((i < DisplayCount) ? items[resultIndex + i].DisplayText : "");
                }
            }
        }

        public void UpdateVisibility(bool visible)
        {
            if (labels[0].InvokeRequired)
            {
                UpdateDisplayDel d = UpdateDisplay;
                labels[0].Invoke(d, new object[] { visible });
            }
            else
            {
                for (int i = 0; i < maxNumItems; i++)
                {
                    labels[i].Visible = visible;
                }
            }
        }

        public bool SelectNext()
        {
            if (items == null || items.Count <= 1 || CurrentSelectionIndex >= items.Count - 1)
                return false;
            indexOffset++;
            if (indexOffset >= maxNumItems)
            {
                indexOffset = maxNumItems - 1;
                resultIndex++;
                UpdateDisplay(true);
            }
            else
                UpdateDisplay(false);
            if (Sc != null)
                Sc.Invoke(CurrentSelectionIndex);
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
            if (Sc != null)
                Sc.Invoke(CurrentSelectionIndex);
            return true;
        }

        public void UpdateWidth(int windowWidth)
        {
            foreach (Label l in labels)
            {
                if (!Equals(l.Width, (windowWidth - 200) / 2))
                    l.Width = (windowWidth - 200) / 2;
            }
        }
    }
}