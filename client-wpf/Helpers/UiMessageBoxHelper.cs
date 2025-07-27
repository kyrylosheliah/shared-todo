namespace TodoClientWpf.Helpers;

static internal class UiMessageBoxHelper
{
    public static async void Show(string title, string message)
    {
        var uiMessageBox = new Wpf.Ui.Controls.MessageBox
        {
            Title = title,
            Content = message
        };
        _ = await uiMessageBox.ShowDialogAsync();
    }
}
