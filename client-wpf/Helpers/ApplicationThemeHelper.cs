using Microsoft.Win32;
using Wpf.Ui.Appearance;

namespace TodoClientWpf.Helpers;
public static class ApplicationThemeHelper
{
    public static ApplicationTheme GetSystemTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int lightTheme)
            {
                return lightTheme == 0 ? ApplicationTheme.Dark : ApplicationTheme.Light;
            }
        }
        catch { }

        return ApplicationTheme.Light;
    }
}
