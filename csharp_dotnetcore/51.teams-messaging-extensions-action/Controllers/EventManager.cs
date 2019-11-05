
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service;

namespace TeamsMessagingExtensionsAction.Controllers
{
    public class EventManager
    {
        private readonly ITableStoreService _tableStoreService;
        public EventManager(ITableStoreService tableStoreService)
        {
            _tableStoreService = tableStoreService;
        }

        public async Task Add(Event evt)
        {
            var meeting = new Event()
            {
                Id = "event123",
                PublishedChannelId = "1234567890",
                Name = "Testing",
                Description = "TestDescription",
                CreatedDateTime = DateTime.Now,
                StartTime = "Nov 11, 2019",
                EndTime = "Nov 12, 2019"
            };
            var result = await _tableStoreService.AddEventAsync(meeting);
        }

        public async Task<IEnumerable<Event>> GetEventByChannelId(string channelId)
        {
            var result = await _tableStoreService.GetEventsByChannelId(channelId);
            return result;
        }
    }
}
