using System;
using Avalonia;
using EnglishDraughtsProject.Models;
using EnglishDraughtsProject.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EnglishDraughtsProject.Views;
using Serilog;

namespace EnglishDraughtsProject
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
            
            
            try
            {
                var services = ConfigureServiceProvider();
                var builder = BuildAvaloniaApp(services);
                builder.StartWithClassicDesktopLifetime(args); 
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static AppBuilder BuildAvaloniaApp(IServiceProvider serviceProvider) =>
            AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace()
                .AfterSetup(_ =>
                {
                    var logger = serviceProvider.GetRequiredService<ILogger<Program>>(); 
                    logger.LogInformation("[Program] : Application started.");
                });

        private static IServiceProvider ConfigureServiceProvider()
        {
            var services = new ServiceCollection();
            
            services.AddLogging(config =>
            {
                config.AddSerilog();
                config.SetMinimumLevel(LogLevel.Debug); 
            });

            services.AddSingleton<Board>(); 
            services.AddSingleton<AiService>(provider =>
            {
                var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("API key is missing. Set the OPENAI_API_KEY environment variable.");
                }
                var logger = provider.GetRequiredService<ILogger<AiService>>();
                return new AiService(apiKey, logger); 
            });

            services.AddSingleton<GameLogicService>(provider =>
            {
                var board = provider.GetRequiredService<Board>();
                var aiService = provider.GetRequiredService<AiService>();
                return new GameLogicService(board, aiService, provider.GetRequiredService<ILogger<GameLogicService>>());
            });

            services.AddSingleton<AiGameLogicService>(provider =>
            {
                var board = provider.GetRequiredService<Board>();
                // var aiService = provider.GetRequiredService<GameLogicService>();
                return new AiGameLogicService(board, provider.GetRequiredService<ILogger<AiGameLogicService>>());
            });

            services.AddSingleton<BoardView>();
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider(); 
        }
    }
}
