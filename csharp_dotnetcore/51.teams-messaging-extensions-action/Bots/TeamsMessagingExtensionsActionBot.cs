// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TeamsMessagingExtensionsAction.Controllers;
using WCB.TeamMeet.Domain;
using WCB.TeamMeet.Storage.Service;

namespace TeamsMessagingExtensionsAction.Bots
{
    public class TeamsMessagingExtensionsActionBot : TeamsActivityHandler
    {
        private readonly ITableStoreService _tableStoreService;

        public TeamsMessagingExtensionsActionBot(ITableStoreService tableStoreService)
        {
            _tableStoreService = tableStoreService;
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {            
            return await CreateCardCommand(turnContext, action);
            
        }

        private readonly string _path = Path.Combine(".", "Resources", "CardTemplate.json");

        private async Task<MessagingExtensionActionResponse> CreateCardCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            try
            {
                // The user has chosen to create a card by choosing the 'Create Card' context menu command.
                var eventData = ((JObject)action.Data).ToObject<Event>();
                var eventId = Guid.NewGuid().ToString();
                eventData.Id = eventId;                
                eventData.PublishedChannelId = turnContext.Activity.ChannelId;
                eventData.CreatedDateTime = DateTime.Now;
                // await turnContext.SendActivityAsync(JsonConvert.SerializeObject(eventData));

                var responseActivity = Activity.CreateMessageActivity();

                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = new AdaptiveCard("1.0")
                    {
                        Body = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock {Text = "Event: " + eventData.Name, Size = AdaptiveTextSize.Large},
                            new AdaptiveTextBlock
                                {Text = "Time: from " + eventData.StartTime + " to " + eventData.EndTime},
                            new AdaptiveTextBlock {Text = "Total capacity: " + eventData.Capacity},
                        },
                        Height = AdaptiveHeight.Auto,
                        Actions = new List<AdaptiveAction>
                        {
                            new AdaptiveSubmitAction
                            {
                                Type = AdaptiveSubmitAction.TypeName,
                                Title = "Attend",
                                Data = new JObject
                                {
                                    {"action", "true"},
                                    {"eventId", eventId },
                                    {"capacity", eventData.Capacity},
                                }
                            },
                            new AdaptiveSubmitAction
                            {
                                Type = AdaptiveSubmitAction.TypeName,
                                Title = "Not Attend",
                                Data = new JObject
                                {
                                    {"action", "false"},
                                    {"eventId", eventId },
                                    {"capacity", eventData.Capacity},
                                }
                            }
                        }
                    }
                };
                responseActivity.Attachments.Add(attachment);
                var manager = new EventManager(_tableStoreService);
                await manager.Add(eventData);

                await turnContext.SendActivityAsync(responseActivity);

            }
            catch (Exception e)
            {
                await turnContext.SendActivityAsync(e.Message);
            }
            return new MessagingExtensionActionResponse();
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                    }

                    break;

                case ActivityTypes.Message:
                    try
                    {
                        JToken commandToken = JToken.Parse(turnContext.Activity.Value.ToString());
                        string action = commandToken["action"].Value<string>();
                        string eventId = commandToken["eventId"].Value<string>();
                        int capacity = commandToken["capacity"].Value<int>();

                        var response = new EventResponse();
                        response.EventId = Guid.NewGuid().ToString();
                        response.ResponseContent = action == "true" ? 1 : 0;
                        response.ResponseUserId = turnContext.Activity.From.Id;
                        response.ResponseUsesrFirstName = turnContext.Activity.From.Name;
                        response.EventId = eventId;
                        response.ResponseDateTime = DateTime.Now;

                        var yes = $"{turnContext.Activity.From.Name} is attending.";
                        var no = $"{turnContext.Activity.From.Name} is NOT attending.";
                        var manager = new EventResponseManager(_tableStoreService);
                        await manager.Add(response);
                        var responses = await manager.GetResponsesByEventId(eventId);
                        var left = capacity - responses.Count(x => x.ResponseContent == 1);

                        var good = $" Number of spot left: {left}";
                        var waitlist = $" You are in waiting list: {-left}";
                        string returnMessage;
                        if (response.ResponseContent == 1)
                        {
                            returnMessage = yes;
                            if (left < 0)
                            {
                                returnMessage += waitlist;
                            }
                            else
                            {
                                returnMessage += good;
                            }
                        }
                        else
                            returnMessage = no;

                        await turnContext.SendActivityAsync(returnMessage,
                            cancellationToken: cancellationToken);

                        //Update primary reply message
                        var responseNames = responses.Where(x => x.ResponseContent == 1).Select(x => x.ResponseUsesrFirstName).ToArray();
                        var nameString = string.Join(",", responseNames);
                        var replyActivity = MessageFactory.Text("Currently enrolled: " + nameString);
                        await turnContext.SendActivityAsync(replyActivity, cancellationToken);

                    }
                    catch (Exception ex)
                    {
                        await turnContext.SendActivityAsync(ex.Message, cancellationToken: cancellationToken);
                    }

                    break;
            }
        }
    }
}
