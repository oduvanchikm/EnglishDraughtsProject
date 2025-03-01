using Avalonia;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EnglishDraughtsProject;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var services = ConfigureServiceProvider();

        var addBuilder = BuildAvaloniaApp(services);

        addBuilder.StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider) =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(_ =>
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogInformation("Application started.");
            });

    private static IServiceProvider ConfigureServiceProvider()
    {
        var services = new ServiceCollection();

        services.AddLogging(config => { config.AddConsole(); });


        return services.BuildServiceProvider();
    }
}