using System;

namespace WCB.TeamMeet.Domain
{
    public class EventResponse
    {
        public string EventId { get; set; }
        public int ResponseContent { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public string ResponseUserId { get; set; }
        public string ResponseUsesrFirstName { get; set; }
        public string ResponseUserLastName { get; set; }
    }
}