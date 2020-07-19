using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using MvcChatBot.Hubs;
using MvcChatBot.Services;


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
            
            
            var host = CreateHostBuilder(args).Build();
            var hubContext = host
                .Services.GetService(typeof(IHubContext<ChatHub>));
            host.RunAsync();
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52179/ChatHub")
                .WithAutomaticReconnect()
                .Build();
           

            IServiceCollection services = new ServiceCollection();
           
            TwitchSettings twitchSettings = new TwitchSettings
            {
                BotName = Configuration.GetValue<string>("TwitchSettings:BotName"),
                AuthToken = Configuration.GetValue<string>("TwitchSettings:AuthToken"),
                Channel = Configuration.GetValue<string>("TwitchSettings:Channel")
            };
           
            services.AddSingleton(twitchSettings);
            var bot = new Bot(twitchSettings, (IHubContext<ChatHub>)hubContext, connection);
            services.AddSingleton<Bot>();
            

            var serviceProvider = services.BuildServiceProvider();
            //Testing writing to line
            Console.WriteLine("Hello World!");
            //IHubContext<ChatHub> context = serviceProvider.GetService<IHubContext<ChatHub>>();
            //Bot twitchChatBot = serviceProvider.GetService<Bot>();
            
            Console.ReadLine();
            

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
       


    }
}