using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace InteractionFramework
{
    public class Program
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true,
        };

        public Program()
        {
            _configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "DC_")
                // Or you can use appsettings.yml. For Example: .AddYmlFile("appsettings.yml", optional: true).
                .AddJsonFile("appsettings.json", optional: true) 
                .Build();

            _services = new ServiceCollection()
                .AddSingleton(_configuration)
                .AddSingleton(_socketConfig)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
                .AddSingleton<InteractionHandler>()
                .BuildServiceProvider();
        }

        static void Main(string[] args)
            => new Program().RunAsync()
                .GetAwaiter()
                .GetResult();

        public async Task RunAsync()
        {
            var client = _services.GetRequiredService<DiscordSocketClient>();

            client.Log += LogAsync;

            // Here we can initialize the service that will register and execute our commands
            await _services.GetRequiredService<InteractionHandler>()
                .InitializeAsync();

            // Bot token can be provided from the Configuration object we set up earlier
            await client.LoginAsync(TokenType.Bot, _configuration["token"]);
            await client.StartAsync();

            // Never quit the program until manually forced to.
            await Task.Delay(Timeout.Infinite);
            
            // Bot status that changes aftera specified time.
            while(true)
            {
                   await _client.SetGameAsync("I love Discord");
                    Thread.sleep(5000);
                    await _client.SetGameAsync("I love Discord.NET");
                    Thread.sleep(5000);

            }
        }

        private async Task LogAsync(LogMessage message)
            => Console.WriteLine(message.ToString());

        public static bool IsDebug()
        {
            #if DEBUG
                return true;
            #else
                return false;
            #endif
        }
    }
}
