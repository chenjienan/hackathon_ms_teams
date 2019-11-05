using System.Collections.Generic;
using System.Threading.Tasks;
using WCB.TeamMeet.Domain;

namespace WCB.TeamMeet.Storage.Service
{
    public interface ITableStoreService
    {
        Task<bool> AddEventAsync(Event meeting);
        Task<bool> AddEventResponseAsync(EventResponse eventResponse);
        Task<Event> GetEventById(string eventId, string channelId);
        Task<IEnumerable<Event>> GetEventsByChannelId(string channelId);
        Task<List<EventResponse>> GetEventResponsesByEventId(string eventId);
    }
}
