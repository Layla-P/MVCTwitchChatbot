using System.Collections.Generic;

namespace MvcChatBot.Agent.Models
{
    public class TrelloSettings
    {
        public string ApiKey { get; set; }
        public string Token { get; set; }

        public string BoardId { get; set; }
        public IEnumerable<TrelloList> TrelloLists { get; set; }
    }
}
