using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WCB.TeamMeet.Storage.Service.Model
{
    public class EventResponseEntity: TableEntity
    {
        public EventResponseEntity(string responseUserId, string eventId)
        {
            PartitionKey = $"{eventId}";
            RowKey = $"{eventId}-{responseUserId}";
        }

        public EventResponseEntity() { }

        public string EventId { get; set; }
        public int ResponseContent { get; set; }
        public DateTime ResponseDateTime { get; set; }
        public string ResponseUserId { get; set; }
        public string ResponseUsesrFirstName { get; set; }
        public string ResponseUserLastName { get; set; }
    }
}