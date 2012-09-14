namespace Multibox.Core.UI
{
    public interface IMainClass {
        IPrefs PreferencesDialog { get; }
        IHelp HelpDialog { get; }
        ILabelManager LabelManager { get; }
        IVersionCheck VChk { get; }
        string InputFieldText { get; set; }
        string OutputLabelText { get; set; }
        string DetailsLabelText { get; set; }
        void UpdateSize();
    }
}