using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service.Model;

namespace WCB.TeamMeet.Storage.Service
{
    public class TableStoreService : ITableStoreService
    {
        //private readonly IMeetingStorageConverter _converter;
        private readonly ICloudStorageAccountWrapper _cloudStorageAccountWrapper;
        private CloudTable _cloudTableEvent;
        private CloudTable _cloudTableResponse;

        public TableStoreService(ICloudStorageAccountWrapper cloudStorageAccountWrapper)
        {
            _cloudStorageAccountWrapper = cloudStorageAccountWrapper;
        }

        private EventEntity CreateEventEntity(Event meeting)
        {
            return new EventEntity(meeting.Id, meeting.PublishedChannelId)
            {
                Id = meeting.Id,
                Name = meeting.Name,
                StartTime = meeting.StartTime,
                EndTime = meeting.EndTime,
                Location = meeting.Location,
                CreatedDateTime = meeting.CreatedDateTime,
                PublishedChannelId = meeting.PublishedChannelId,
                Capacity = meeting.Capacity
            };
        }

        private EventResponseEntity CreateEventResponseEntity(EventResponse response)
        {
            return new EventResponseEntity(response.ResponseUserId, response.EventId)
            {
                EventId = response.EventId,
                ResponseDateTime = response.ResponseDateTime,
                ResponseUserId = response.ResponseUserId,
                ResponseUserLastName = response.ResponseUserLastName,
                ResponseUsesrFirstName = response.ResponseUsesrFirstName,
                ResponseContent = response.ResponseContent
            };
        }

        private async Task<bool> ExecuteEventInsertAsync(ITableEntity meetingEntity)
        {
            if (!await InitializeEventTableReference())
            {
                return false;
            }

            var insertOperation = TableOperation.InsertOrReplace(meetingEntity);
            var tableExecuteResult = await _cloudTableEvent.ExecuteAsync(insertOperation);

            return tableExecuteResult.HttpStatusCode < 300;
        }
        private async Task<bool> ExecuteResponseInsertAsync(ITableEntity responseEntity)
        {
            if (!await InitializeResponseTableReference())
            {
                return false;
            }

            var insertOperation = TableOperation.InsertOrReplace(responseEntity);
            var tableExecuteResult = await _cloudTableResponse.ExecuteAsync(insertOperation);

            return tableExecuteResult.HttpStatusCode < 300;
        }

        private async Task<bool> InitializeEventTableReference()
        {
            if (_cloudTableEvent == null)
            {
                _cloudTableEvent = _cloudStorageAccountWrapper.GetTableClient.GetTableReference("WcbEvent");
            }

            if (await _cloudTableEvent.ExistsAsync()) return true;
            var isSuccessful = await _cloudTableEvent.CreateIfNotExistsAsync();

            return isSuccessful;
        }

        private async Task<bool> InitializeResponseTableReference()
        {
            if (_cloudTableResponse == null)
            {
                _cloudTableResponse = _cloudStorageAccountWrapper.GetTableClient.GetTableReference("WcbEventResponse");
            }

            if (await _cloudTableResponse.ExistsAsync()) return true;
            var isSuccessful = await _cloudTableResponse.CreateIfNotExistsAsync();

            return isSuccessful;
        }
        public Task<bool> AddEventAsync(Event meeting)
        {
            var eventEntity = CreateEventEntity(meeting);
            return ExecuteEventInsertAsync(eventEntity);
        }

        public Task<bool> AddEventResponseAsync(EventResponse eventResponse)
        {

            var eventEntity = CreateEventResponseEntity(eventResponse);
            return ExecuteResponseInsertAsync(eventEntity);
        }

        public async Task<Event> GetEventById(string eventId, string channelId)
        {
            //try to retrieve a single entity
            try
            {
                var retrieveOperation = TableOperation.Retrieve<EventEntity>($"eventId", $"{channelId}");

                var eventResult = await _cloudTableEvent.ExecuteAsync(retrieveOperation);

                if (eventResult != null && eventResult.HttpStatusCode < 300)
                {
                    var result = (EventEntity)eventResult.Result;

                    return new Event
                    {
                        Id = result.Id,
                        Location = result.Location,
                        Name = result.Name,
                        StartTime = result.StartTime,
                        EndTime = result.EndTime,
                        CreatedDateTime = result.CreatedDateTime,
                        Capacity = result.Capacity,
                        PublishedChannelId = result.PublishedChannelId
                    };
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        public async Task<IEnumerable<Event>> GetEventsByChannelId(string channelId)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<EventEntity>()
                            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, $"{channelId}"));

            TableContinuationToken token = null;
            var result = new List<Event>();

            if (!await InitializeEventTableReference())
            {
                return result;
            }

            do
            {
                var resultSegment = await _cloudTableEvent.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                if (resultSegment.Results.Any())
                {
                    result.AddRange(resultSegment.Results.Select(x =>
                        new Event
                        {
                            Id = x.Id,
                            Location = x.Location,
                            Name = x.Name,
                            StartTime = x.StartTime,
                            EndTime = x.EndTime,
                            Capacity = x.Capacity,
                            CreatedDateTime = x.CreatedDateTime,
                            PublishedChannelId = x.PublishedChannelId
                        }));
                }

            } while (token != null);

            return result;
        }

        public async Task<List<EventResponse>> GetEventResponsesByEventId(string eventId)
        {
            // Construct the query operation for all customer entities where PartitionKey="Smith".
            var query = new TableQuery<EventResponseEntity>()
                            .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal,
                                                                      $"{eventId}"));

            TableContinuationToken token = null;
            var result = new List<EventResponse>();

            if (!await InitializeResponseTableReference())
            {
                return result;
            }

            do
            {
                var resultSegment = await _cloudTableResponse.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                result.AddRange(resultSegment
                                           .Results
                                           .Select(x => new EventResponse
                                           {
                                               EventId = x.PartitionKey,
                                               ResponseUserId = x.ResponseUserId,
                                               ResponseUserLastName= x.ResponseUserLastName,
                                               ResponseUsesrFirstName = x.ResponseUsesrFirstName,
                                               ResponseDateTime = x.ResponseDateTime.ToLocalTime(),
                                               ResponseContent = x.ResponseContent
                                           }));

            } while (token != null);

            return result;
        }
    }
}
