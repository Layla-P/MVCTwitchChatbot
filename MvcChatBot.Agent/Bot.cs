using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using MvcChatBot.Hubs;
using MvcChatBot.Services;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace MvcChatBot.Agent
{
    public class Bot
    {
        private readonly TwitchClient _client;
        private readonly TwitchSettings _settings;
        private readonly IHubContext<ChatHub> _hub;
        private readonly HubConnection _connection;


        public Bot(
            TwitchSettings settings,
            IHubContext<ChatHub> hub,
            HubConnection connection)
        {
            _settings = settings;
            _hub = hub;
            _connection = connection;
            _connection.StartAsync();
            
           
            
            ConnectionCredentials credentials = new ConnectionCredentials(_settings.BotName, _settings.AuthToken);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            _client = new TwitchClient(customClient);
            _client.Initialize(credentials, _settings.Channel);

            _client.OnLog += Client_OnLog;
            _client.OnJoinedChannel += Client_OnJoinedChannel;
            _client.OnMessageReceived += async (s, e) => { await Client_OnMessageReceived(s, e); };
            _client.OnWhisperReceived += Client_OnWhisperReceived;
            _client.OnNewSubscriber += Client_OnNewSubscriber;
            _client.OnConnected += Client_OnConnected;

            _client.Connect();
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine("Hey guys! I am a bot connected via TwitchLib!");
            _client.SendMessage(e.Channel, "Hey guys! I am a bot connected via TwitchLib!");
        }

        private async Task Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            if (e.ChatMessage.Message.StartsWith("!rain", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(_connection.ConnectionId);
                
                await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName,"Make it rain!!!");
                
               // await _hub.Clients.All.SendAsync("TriggerRain");
                //_hub.Clients.All.SendAsync("TriggerRain");
                //_client.SendMessage(_settings.Channel, $"Hey there { e.ChatMessage.DisplayName }.");
            }

            // else if (e.ChatMessage.Message.StartsWith("!uptime", StringComparison.InvariantCultureIgnoreCase))
            // {
            //     var upTime = GetUpTime().Result;
            //
            //     _client.SendMessage(_settings.Channel, upTime?.ToString() ?? "Offline");
            // }
        }

        private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
        {
            if (e.WhisperMessage.Username == "my_friend")
                _client.SendWhisper(e.WhisperMessage.Username, "Hey! Whispers are so cool!!");
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                _client.SendMessage(e.Channel,
                    $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                _client.SendMessage(e.Channel,
                    $"Welcome {e.Subscriber.DisplayName} to the substers! You just earned 500 points!");
        }
    }
}