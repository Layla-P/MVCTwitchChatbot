using System;
using System.Collections.Generic;
using System.Text;

namespace MvcChatBot.Agent.Models
{
    public class NewTrelloCard
    {
        public string UserName { get; set; }
        public string ListName { get; set; }
        public string CardName { get; set; }
        public string Description { get; set; }
    }
}
