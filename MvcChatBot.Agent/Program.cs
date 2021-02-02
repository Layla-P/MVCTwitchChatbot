using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using MvcChatBot.Agent.Services;
using MvcChatBot.Agent.Models;
using System.Collections.Generic;
using System.Linq;

namespace MvcChatBot.Agent
{
   
    class Program
    {
        static void Main(string[] args)
        {
            IConfiguration Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .AddCommandLine(args)
                .Build();
            
 
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:52172/ChatHub")
                .WithAutomaticReconnect()
                .Build();
           

            IServiceCollection services = new ServiceCollection();

            var lists = new List<TrelloList>();
            var test = Configuration.GetSection("TrelloSettings:TrelloLists")
                .GetChildren().ToList();

            foreach (var l in test)
            {
                var list = new TrelloList();
                l.Bind(list);
                lists.Add(list);
            }

            TrelloSettings trelloSettings = new TrelloSettings
            {
                ApiKey = Configuration.GetValue<string>("TrelloSettings:ApiKey"),
                Token = Configuration.GetValue<string>("TrelloSettings:Token"),
                BoardId = Configuration.GetValue<string>("TrelloSettings:BoardId"),
                TrelloLists = lists
            };


            services.AddSingleton(trelloSettings);
            services.AddSingleton<TrelloService>();

            TwitchSettings twitchSettings = new TwitchSettings
            {
                BotName = Configuration.GetValue<string>("TwitchSettings:BotName"),
                AuthToken = Configuration.GetValue<string>("TwitchSettings:AuthToken"),
                Channel = Configuration.GetValue<string>("TwitchSettings:Channel"),
                ChannelId = Configuration.GetValue<string>("TwitchSettings:ChannelId"),
                ChannelAuthToken = Configuration.GetValue<string>("TwitchSettings:ChannelAuthToken"),
                ClientId = Configuration.GetValue<string>("TwitchSettings:ClientId")
            };
           
            services.AddSingleton(twitchSettings);
            services.AddSingleton(connection);
            services.AddSingleton<TwitchClientService>();

            services.AddSingleton<TwitchPubSubService>();

            var serviceProvider = services.BuildServiceProvider();
            var twitchClientService = serviceProvider.GetService<TwitchClientService>();
            //api service not currently in use
            //var twitchApiService = serviceProvider.GetService<TwitchApiService>();
            var twitchPubSubService = serviceProvider.GetService<TwitchPubSubService>();


            Console.WriteLine("Hello World!");

            Console.ReadLine();
            

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.UseStartup<Startup>();
                });
       


    }
}