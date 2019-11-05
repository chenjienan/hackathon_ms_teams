using System;
using System.Collections.Generic;

namespace WCB.TeamMeet.Domain
{
    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int MinCapacity { get; set; }
        public int MaxCapacity { get; set; }
        public string PublishedChannelId { get; set; }
        //public List<EventResponse> Responses { get; set; }
    }
}
