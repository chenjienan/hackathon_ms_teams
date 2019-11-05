using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service;

namespace TeamsMessagingExtensionsAction.Controllers
{
    [ApiController]
    public class EventResponseController : ControllerBase
    {
        private readonly ITableStoreService _tableStoreService;
        public EventResponseController(ITableStoreService tableStoreService)
        {
            _tableStoreService = tableStoreService;
        }

        [Route("api/responses/Add")]
        [HttpPost]
        public async Task Add()
        {
            var response = new EventResponse()
            {
                ResponseUserId = "nick123",
                EventId = "event123",
                ResponseUserLastName = "Chang",
                ResponseUsesrFirstName = "Nick",
                ResponseContent =  1,
                ResponseDateTime = DateTime.Now
            };
            var result = await _tableStoreService.AddEventResponseAsync(response);
        }

        [Route("api/responses/GetByEventId")]
        [HttpPost]
        public async Task<List<EventResponse>> GetResponsesByEventId(string eventId)
        {
            var result = await _tableStoreService.GetEventResponsesByEventId(eventId);
            return result;
        }
    }
}
