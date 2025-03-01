using Avalonia;
using System;
using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;
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

    private static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider) =>
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
        
        services.AddSingleton<Board>();
        
        services.AddSingleton<AiService>(provider =>
        {
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine($"✅ API-ключ загружен: {apiKey.Substring(0, 5)}********");
                throw new InvalidOperationException("DeepSeek API key is missing. Set the OPENAI_API_KEY environment variable.");
            }
            
            Console.WriteLine($"✅ API-ключ загружен: {apiKey.Substring(0, 5)}********");

            var logger = provider.GetRequiredService<ILogger<AiService>>();
            return new AiService(apiKey, logger);
        });

        services.AddSingleton<GameLogicService>(provider =>
        {
            var board = provider.GetRequiredService<Board>();
            var aiService = provider.GetRequiredService<AiService>();
            return new GameLogicService(board, aiService);
        });
        
        return services.BuildServiceProvider();
    }
}