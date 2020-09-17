using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using MvcChatBot.Hubs;
using MvcChatBot.Services;
//using MvcChatBot.Agent.Services;

namespace MvcChatBot.Agent
{
   
    class Program
    { 
       
        
        // //credentials (suppressed for privacy)
        // private static string login_name = "<LOGIN_NAME>";
        // private static string token = Environment.GetEnvironmentVariable("Token");  //Token should be stored in a safe place
        // private static List<string> channels_to_join = new List<string>(new string[] { "<CHANNEL_1>", "<CHANNEL_2>" });

        //main function
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();
            
 
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52179/ChatHub")
                .WithAutomaticReconnect()
                .Build();
           

            IServiceCollection services = new ServiceCollection();
           
            TwitchSettings twitchSettings = new TwitchSettings
            {
                BotName = Configuration.GetValue<string>("TwitchSettings:BotName"),
                AuthToken = Configuration.GetValue<string>("TwitchSettings:AuthToken"),
                Channel = Configuration.GetValue<string>("TwitchSettings:Channel"),
                ChannelId = Configuration.GetValue<string>("TwitchSettings:ChannelId"),
                ChannelAuthToken = Configuration.GetValue<string>("TwitchSettings:ChannelAuthToken")
            };
           
            services.AddSingleton(twitchSettings);
            var bot = new Bot(twitchSettings, connection);
            services.AddSingleton(bot);

            //var pubsubService = new TwitchPubSubService(twitchSettings, connection);
            //services.AddSingleton(pubsubService);


            var serviceProvider = services.BuildServiceProvider();

            Console.WriteLine("Hello World!");
            
            var command = Console.ReadLine();
            
            if(command == "raid")
            {
                bot.TestRaid();
            }

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
       


    }
}