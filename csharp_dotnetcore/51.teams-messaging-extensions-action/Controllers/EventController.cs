
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service;

namespace TeamsMessagingExtensionsAction.Controllers
{
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ITableStoreService _tableStoreService;
        public EventController(ITableStoreService tableStoreService)
        {
            _tableStoreService = tableStoreService;
        }

        [Route("api/events/Add")]
        [HttpPost]
        public async Task Add()
        {
            var meeting = new Event()
            {
                Id = "event123",
                PublishedChannelId = "1234567890",
                Name = "Testing",
                CreatedDateTime = DateTime.Now,
                StartTime = "Nov 11, 2019",
                EndTime = "Nov 12, 2019"
            };
            var result = await _tableStoreService.AddEventAsync(meeting);
        }

        [Route("api/events/GetEventByChannelId")]
        [HttpPost]
        public async Task<IEnumerable<Event>> GetEventByChannelId(string channelId)
        {
            var result = await _tableStoreService.GetEventsByChannelId(channelId);
            return result;
        }
    }
}
