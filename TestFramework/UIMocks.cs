using System.Collections.Generic;
using Multibox.Core.UI;

namespace Multibox.Test.TestFramework
{
    internal class MockMainClass : IMainClass
    {
        public IPrefs PreferencesDialog { get; private set; }
        public IHelp HelpDialog { get; private set; }
        public ILabelManager LabelManager { get; private set; }
        public IVersionCheck VChk { get; private set; }
        public string InputFieldText { get; set; }
        public string OutputLabelText { get; set; }
        public string DetailsLabelText { get; set; }
        public void UpdateSize() {}

        public MockMainClass(int m)
        {
            PreferencesDialog = new MockPrefs();
            HelpDialog = new MockHelp();
            LabelManager = new MockLabelManager(m);
            VChk = new MockVersionCheck();
            InputFieldText = "";
            OutputLabelText = "";
            DetailsLabelText = "";
        }
    }

    internal class MockPrefs : IPrefs {}

    internal class MockHelp : IHelp
    {
        public Dictionary<string, List<ResultItem>> AutocompleteOptions { get; private set; }

        public void ShowPage(string page) {}

        public void LaunchPage(string page) {}

        public List<ResultItem> GetAutocompleteOptions(string path)
        {
            return (AutocompleteOptions.ContainsKey(path) ? AutocompleteOptions[path] : null);
        }

        public MockHelp()
        {
            AutocompleteOptions = new Dictionary<string, List<ResultItem>>(0);
        }
    }

    internal class MockLabelManager : ILabelManager
    {
        private List<ResultItem> items;
        private int resultIndex = -1;
        private int indexOffset;
        private static int maxNumItems = 10;
        public SelectionChanged Sc { get; set; }

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
                catch {}
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
                if (Sc != null)
                    Sc.Invoke(CurrentSelectionIndex);
            }
        }

        public MockLabelManager(int m)
        {
            maxNumItems = m;
        }

        public void UpdateDisplay(bool updateText) {}

        public void UpdateVisibility(bool visible) {}

        public bool SelectNext()
        {
            if (items == null || items.Count <= 1 || CurrentSelectionIndex >= items.Count - 1)
                return false;
            indexOffset++;
            if (indexOffset >= maxNumItems)
            {
                indexOffset = maxNumItems - 1;
                resultIndex++;
            }
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
            }
            if (Sc != null)
                Sc.Invoke(CurrentSelectionIndex);
            return true;
        }

        public void UpdateWidth(int windowWidth) {}
    }

    internal class MockVersionCheck : IVersionCheck
    {
        public void CheckForUpdateForce() {}
    }
}