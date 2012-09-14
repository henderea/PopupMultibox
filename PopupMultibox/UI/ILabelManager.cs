using System.Collections.Generic;

namespace Multibox.Core.UI
{
    public interface ILabelManager {
        SelectionChanged Sc { get; set; }
        int ResultIndex { get; }
        int CurrentSelectionIndex { get; }
        ResultItem CurrentSelection { get; }
        int DisplayCount { get; }
        int ResultHeight { get; }
        int WindowHeight { get; }
        List<ResultItem> ResultItems { get; set; }
        void UpdateDisplay(bool updateText);
        void UpdateVisibility(bool visible);
        bool SelectNext();
        bool SelectPrev();
        void UpdateWidth(int windowWidth);
    }
}