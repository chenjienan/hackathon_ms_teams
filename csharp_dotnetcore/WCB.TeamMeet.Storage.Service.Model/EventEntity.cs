using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WCB.TeamMeet.Storage.Service.Model
{
    public class EventEntity : TableEntity
    {
        public EventEntity(string eventId, string channelId)
        {
            PartitionKey = $"{channelId}";
            RowKey = $"{eventId}";
        }

        public EventEntity() { }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }
        public string PublishedChannelId { get; set; }
    }
}
