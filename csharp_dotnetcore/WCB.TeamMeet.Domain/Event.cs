using System;
using System.Collections.Generic;

namespace WCB.TeamMeet.Domain
{
    public class Event
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string PublishedChannelId { get; set; }
        //public List<EventResponse> Responses { get; set; }
    }
}
