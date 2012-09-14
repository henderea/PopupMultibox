using System.Collections.Generic;

namespace Multibox.Core.UI
{
    public interface IHelp {
        void ShowPage(string page);
        void LaunchPage(string page);
        List<ResultItem> GetAutocompleteOptions(string path);
    }
}