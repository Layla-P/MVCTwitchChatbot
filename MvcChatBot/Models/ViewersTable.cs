using System.Collections.Generic;
using Newtonsoft.Json;

namespace MvcChatBot.Models
{
    public class ViewersTable
    {
        public string Name { get; set; }
        [JsonProperty("Preferred Pronoun")]
        public string PreferredPronoun { get; set; }
        [JsonProperty("Email Address")]
        public string EmailAddress { get; set; }
        // [JsonProperty("Which country are you viewing from?")]
        // public string Country { get; set; }
        [JsonProperty("Which streamers do you watch on Twilio TV?")]
        public List<string> StreamersWatched { get; set; }
        //public string MailingAddress { get; set; }
    }
}