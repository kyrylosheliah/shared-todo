using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Interop;
using TodoClientWpf.Helpers;
using TodoClientWpf.Services;
using TodoClientWpf.Views;
using Wpf.Ui.Appearance;
using Wpf.Ui.Markup;

namespace TodoClientWpf;

public partial class App : Application
{
    public static IHost AppHost { get; private set; } = Host.CreateDefaultBuilder()
        .ConfigureServices((hostContent, services) => {
            services.AddHttpClient();
            services.AddSingleton<TaskWsService>();
            services.AddSingleton<TaskCrudService>();
            services.AddSingleton<MainWindow>();
        })
        .Build();

    public App() {}

    protected override async void OnStartup(StartupEventArgs e)
    {
        ApplySystemTheme();
        await AppHost.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);

        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category == UserPreferenceCategory.General)
        {
            ApplySystemTheme();
        }
    }
    private void ApplySystemTheme()
    {
        var newTheme = ApplicationThemeHelper.GetSystemTheme();
        ApplicationThemeManager.Apply(newTheme);
    }
}

