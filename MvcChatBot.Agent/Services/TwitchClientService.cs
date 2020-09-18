using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using MvcChatBot.Agent.Models;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;


namespace MvcChatBot.Agent.Services
{
    public class TwitchClientService
    {
        private readonly TwitchClient _client;
        private readonly TwitchSettings _settings;
        private readonly HubConnection _connection;
        private readonly TrelloService _trelloService;


        public TwitchClientService(
            TwitchSettings settings,
            HubConnection connection,
            TrelloService trelloService)
        {
            _settings = settings;
            _connection = connection;
            _trelloService = trelloService;
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
            _client.OnChatCommandReceived += Client_OnCommandReceived;
            _client.OnWhisperReceived += Client_OnWhisperReceived;
            _client.OnRaidNotification += Client_OnRaidNotification;
            _client.OnNewSubscriber += Client_OnNewSubscriber;
            _client.OnConnected += Client_OnConnected;

            _client.Connect();

        }
        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime}: {e.BotUsername} - {e.Data}");
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
        private async void Client_OnCommandReceived(object sender, OnChatCommandReceivedArgs e)
        {

            switch (e.Command.CommandText.ToLower())
            {

                case "trello":
                    _client.SendMessage(_settings.Channel,
                         "Try typing !todo/!general/!bot/!links \"title of card\" \"The description of the card or URL\"");
                    break;
                case "todo":
                    CreateTrelloCard(e.Command, "todo");
                    break;
                case "general":
                    CreateTrelloCard(e.Command, "General Ideas");
                    break;
                case "bot":
                    CreateTrelloCard(e.Command, "Bot Ideas");
                    break;
                case "links":
                    CreateTrelloCard(e.Command, "links");
                    break;
                case "rain":
                    await MakeItRain(e.Command);
                    break;
                case "waffle":
                    await Waffling(e.Command);
                    break;
                case "balls":
                    await PlayBalls(e.Command);
                    break;
                default:
                    break;


            }

        }
        private async Task MakeItRain(ChatCommand e)
        {
            await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "Make it rain!!!", false, false);

        }
        private async Task Waffling(ChatCommand e)
        {
            await _connection.InvokeAsync("SendMessage", e.ChatMessage.DisplayName, "Waffling", false, true);
            _client.SendMessage(e.ChatMessage.Channel, "Layla is waffling!!");
        }
        private async Task PlayBalls(ChatCommand e)
        {
            Console.WriteLine(_connection.ConnectionId);
            if (e.ChatMessage.IsModerator || e.ChatMessage.IsBroadcaster)
            {
                _client.SendMessage(e.ChatMessage.Channel, "Time to get your balls in! Type !prizedraw in the chat to be in with a chance to win!");

                await _connection.InvokeAsync("PlaySoundMessage", e.ChatMessage.DisplayName, "balls");
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
                    $"Welcome {e.Subscriber.DisplayName} to the wafflers! So kind of you to use your Twitch Prime on this channel!");
            else
                _client.SendMessage(e.Channel,
                    $"Welcome {e.Subscriber.DisplayName} to the wafflers!");
        }


        private void CreateTrelloCard(ChatCommand e, string listName)
        {
            try
            {
                var messageArray = CardMessageHandler(e.ArgumentsAsString);
                if (messageArray.Length == 2)
                {
                    if (e.ChatMessage.IsModerator
                       || e.ChatMessage.IsBroadcaster
                       || e.ChatMessage.IsSubscriber
                       || e.ChatMessage.IsVip)
                    {
                        var testCard = new NewTrelloCard
                        {
                            UserName = e.ChatMessage.DisplayName,
                            CardName = messageArray[0],
                            Description = messageArray[1],
                            ListName = listName
                        };
                        _trelloService.AddNewCardAsync(testCard);
                    }
                }
               
            }
            catch (Exception ex)
            {
                _client.SendMessage(_settings.Channel,
                   $"{e.ChatMessage.DisplayName} That card wasn't created, sorry!!");
                Console.WriteLine($"Failed to write Trello card: {ex.Message}");
            }
        }
        private static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString()); DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[]; if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }
            return value.ToString();
        }



        private string[] CardMessageHandler(string message)
        {
            return message.TrimStart('"').TrimEnd('"').Split("\" \"");
        }
    }
}