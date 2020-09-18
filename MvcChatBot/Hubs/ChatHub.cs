using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MvcChatBot.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, bool isSuper = false, bool isWaffle = false)
        {
            await Clients.All.SendAsync("LaylaMessage", user, message, isSuper, isWaffle);
        }

        public async Task Raid(int raiderCount)
        {
            await Clients.All.SendAsync("Bops", raiderCount);
        }

        public async Task PlaySoundMessage(string user, string message)
        {
            await Clients.All.SendAsync("SoundMessage", user, message);
        }

        public async Task UpdateBrowser()
        {
            
                await Clients.All.SendAsync("TriggerRain");
            
            
        }
        
        public Task SendMessageToGroup(string message)
        {
            return Clients.Group("SignalR Users").SendAsync("ReceiveMessage", message);
        }
    }
}