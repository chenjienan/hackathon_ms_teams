﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
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
        //private readonly ITableStoreService _tableStoreService;

        //public TeamsMessagingExtensionsActionBot(ITableStoreService tableStoreService)
        //{
        //    _tableStoreService = tableStoreService;
        //}

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
                //var eventData = ((JObject)action.Data).ToObject<Event>();

                var responseActivity = Activity.CreateMessageActivity();
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = new AdaptiveCard("1.0")
                    {
                        Body = new List<AdaptiveElement>()
                        {
                            new AdaptiveTextBlock { Text = "FormField1 value was:", Size = AdaptiveTextSize.Large },
                            //new AdaptiveTextBlock { Text = eventData.Name }
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
                                    { "action", "true" } 

                                }                                
                            }
                        }
                    }
                };
                responseActivity.Attachments.Add(attachment);

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
            DialogContext dc = null;
            string adaptiveCard = string.Empty;

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
                        await turnContext.SendActivityAsync(turnContext.Activity.ToString(), cancellationToken: cancellationToken);

                        var token = JToken.Parse(turnContext.Activity.ChannelData.ToString());
                        string selectedcolor = string.Empty;
                        var response = new EventResponse();
                        if (Convert.ToBoolean(token["postback"].Value<string>()))
                        {
                            //JToken commandToken = JToken.Parse(turnContext.Activity.Value.ToString());
                            //string action = commandToken["action"].Value<string>();

                            response.EventId = Guid.NewGuid().ToString();
                            response.ResponseContent = 1;
                            response.ResponseUserId = turnContext.Activity.From.Id;
                            response.ResponseUsesrFirstName = turnContext.Activity.From.Name;
                        }

                        var yes = $"{turnContext.Activity.From.Name} is attending";
                        var no = $"{turnContext.Activity.From.Name} is NOT attending";

                        //var manager = new EventResponseManager(_tableStoreService);
                        //await manager.Add(response);

                        //await turnContext.SendActivityAsync(response.ResponseContent == 1 ? yes : no,
                        //    cancellationToken: cancellationToken);
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
