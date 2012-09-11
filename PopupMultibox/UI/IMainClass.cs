namespace Multibox.Core.UI
{
    public interface IMainClass {
        Prefs PreferencesDialog { get; }
        Help HelpDialog { get; }
        LabelManager LabelManager { get; }
        VersionCheck VChk { get; }
        string HomeDirectory { get; }
        string InputFieldText { get; set; }
        string OutputLabelText { get; set; }
        string DetailsLabelText { get; set; }
        void UpdateSize();
    }
}