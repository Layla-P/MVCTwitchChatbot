using System.Linq;
using MvcChatBot.Agent.Models;
using TrelloNet;

namespace MvcChatBot.Agent.Services
{
    public class TrelloService
    {
        private readonly TrelloSettings _trelloSettings;
        private readonly ITrello _trello;

        public TrelloService(TrelloSettings trelloSettings)
        {
            _trelloSettings = trelloSettings;

            _trello = new Trello(_trelloSettings.ApiKey);

            _trello.Authorize(_trelloSettings.Token);

        }


        public void AddNewCardAsync(NewTrelloCard card)
        {
            var list = _trelloSettings
                .TrelloLists.FirstOrDefault(l => l.Name.ToLower() == card.ListName.ToLower());
            var listActual = _trello.Lists.WithId(list.Id);
            var board = _trello
                .Boards.WithId(_trelloSettings.BoardId);

            Card trelloCard = _trello
                .Cards.Add(new NewCard(card.CardName, listActual));

            trelloCard.Desc = $"{card.UserName} suggests {card.Description}";

            _trello.Cards.Update(trelloCard);
        }
    }
}
