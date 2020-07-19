using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;
using Microsoft.Extensions.Options;
using MvcChatBot.Models;


namespace MvcChatBot.Services
{
    public class AirtableService
    {
        private readonly string _baseId;
        private readonly string _appKey;


        public AirtableService(IOptions<AirtableSettings> settings)
        {
            _appKey = settings.Value.ApiKey;
            _baseId = settings.Value.BaseId;
        }

        public async Task<int> GetCount()
        {
            int count = 0;

            using (AirtableBase airtableBase = new AirtableBase(_appKey, _baseId))
            {
                Task<AirtableListRecordsResponse<ViewersTable>> task = airtableBase.ListRecords<ViewersTable>(
                    "Viewers");

                AirtableListRecordsResponse<ViewersTable> response = await task;

                if (response.Success)
                {
                    var viewers = new List<ViewersTable>();
                    var records = response.Records.ToList();
                    foreach (var rec in records)
                    {
                        var viewer = new ViewersTable
                        {
                            Name = rec.Fields.Name,
                            PreferredPronoun = rec.Fields.PreferredPronoun,
                            EmailAddress = rec.Fields.EmailAddress,
                            StreamersWatched = rec.Fields.StreamersWatched
                        };
                        viewers.Add(viewer);
                    }

                    var distinctViewers = viewers.GroupBy(g => new {g.EmailAddress});
                    count = distinctViewers.Count();
                }
            }

            return count;
        }
    }
}