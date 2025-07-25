using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using TodoClientWpf.Services;

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
        await AppHost.StartAsync();

        var startupForm = AppHost.Services.GetRequiredService<MainWindow>();
        startupForm.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await AppHost.StopAsync();
        base.OnExit(e);
    }
}

