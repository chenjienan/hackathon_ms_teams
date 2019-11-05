using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service;

namespace TeamsMessagingExtensionsAction.Controllers
{

    public class EventResponseManager
    {
        private readonly ITableStoreService _tableStoreService;
        public EventResponseManager(ITableStoreService tableStoreService)
        {
            _tableStoreService = tableStoreService;
        }

        public async Task Add(EventResponse res)
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


        public async Task<List<EventResponse>> GetResponsesByEventId(string eventId)
        {
            var result = await _tableStoreService.GetEventResponsesByEventId(eventId);
            return result;
        }
    }
}
