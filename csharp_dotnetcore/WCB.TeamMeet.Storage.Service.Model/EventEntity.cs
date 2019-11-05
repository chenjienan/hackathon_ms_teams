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
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Location { get; set; }
        public int MinCapacity { get; set; }
        public int MaxCapacity { get; set; }
        public string PublishedChannelId { get; set; }
    }
}
