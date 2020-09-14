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
        private readonly HubConnection _connection;


        public Bot(
            TwitchSettings settings,
            HubConnection connection)
        {
            _settings = settings;
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
            _client.OnMessageReceived += Client_OnMessageReceived;
            _client.OnWhisperReceived += Client_OnWhisperReceived;
            _client.OnRaidNotification += Client_OnRaidNotification;
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
            _client.SendMessage(e.Channel, "Hello lovelies, I'm Layla's little helper!");
        }

        private async void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            if (e.ChatMessage.Message.StartsWith("!rain", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(_connection.ConnectionId);

                await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "Make it rain!!!", false);

            }

            


            //if (e.ChatMessage.Message.StartsWith("!superrain", StringComparison.InvariantCultureIgnoreCase))
            //{
            //    Console.WriteLine(_connection.ConnectionId);

            //    await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "It's a torrential downpour of destructopups!!!", true);

            //}

            if (e.ChatMessage.Message.StartsWith("!balls", StringComparison.InvariantCultureIgnoreCase))
            {
                Console.WriteLine(_connection.ConnectionId);
                if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
                {
                    _client.SendMessage(e.ChatMessage.Channel, "Time to get your balls in! Type !prizedraw in the chat to be in with a chance to win!");

                    await _connection.InvokeAsync("PlaySoundMessage", e.ChatMessage.DisplayName, "balls");
                }

            }
        }

        private async void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            int.TryParse(e.RaidNotification.MsgParamViewerCount, out var count);

            count = count != 0 ? count : 1;

            await _connection.InvokeAsync("Raid", count);
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