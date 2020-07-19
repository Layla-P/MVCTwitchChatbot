using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcChatBot.Hubs;
using MvcChatBot.Services;

namespace MvcChatBot.Controllers
{
    [ApiController]
    [Route("api/destructopup")]
    public class DestructoPupController :ControllerBase
    {
        private readonly BroadcastService _broadcastService;
        public DestructoPupController(BroadcastService broadcastService)
        {
            _broadcastService = broadcastService;
        }
        
        
        [HttpGet]
        public async Task Get()
        {
             _broadcastService.SendNotificationAsync();
           
        }
    }
}