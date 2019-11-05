// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsMessagingExtensionsActionBot : TeamsActivityHandler
    {
        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            switch (action.CommandId)
            {
                // These commandIds are defined in the Teams App Manifest.
                case "createCard":
                    return await CreateCardCommand(turnContext, action);

                case "shareMessage":
                    return ShareMessageCommand(turnContext, action);
                default:
                    throw new NotImplementedException($"Invalid CommandId: {action.CommandId}");
            }
        }

        private readonly string _path = Path.Combine(".", "Resources", "CardTemplate.json");

        private async Task<MessagingExtensionActionResponse> CreateCardCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // The user has chosen to create a card by choosing the 'Create Card' context menu command.
            var eventData = ((JObject)action.Data).ToObject<Event>();

            //var card = new HeroCard
            //{
            //    Title = createCardData.Title,
            //    Subtitle = createCardData.Subtitle,
            //    Text = createCardData.Text,
            //};

            //var attachments = new List<MessagingExtensionAttachment>();
            //attachments.Add(new MessagingExtensionAttachment
            //{
            //    Content = card,
            //    ContentType = HeroCard.ContentType,
            //    Preview = card.ToAttachment(),
            //});

            var adaptiveCardJson = File.ReadAllText(_path);

            //dynamic Data = JObject.Parse(action.Data.ToString());
            //var response = new MessagingExtensionActionResponse
            //{
            //    ComposeExtension = new MessagingExtensionResult
            //    {
            //        Type = "result",
            //        ActivityPreview = MessageFactory.Attachment(new Attachment
            //        {
            //            Content = new AdaptiveCard("1.0")
            //            {
            //                Body = new List<AdaptiveElement>()
            //                {
            //                    new AdaptiveTextBlock() { Text = "FormField1 value was:", Size = AdaptiveTextSize.Large },
            //                    new AdaptiveTextBlock() { Text = Data["FormField1"] as string }
            //                },
            //                Height = AdaptiveHeight.Auto,
            //                Actions = new List<AdaptiveAction>()
            //                {
            //                    new AdaptiveSubmitAction
            //                    {
            //                        Type = AdaptiveSubmitAction.TypeName,
            //                        Title = "Submit",
            //                        Data = new JObject { { "submitLocation", "messagingExtensionFetchTask" } },
            //                    },
            //                }
            //            },
            //            ContentType = AdaptiveCard.ContentType
            //        }) as Activity
            //    }
            //};


            var responseActivity = Activity.CreateMessageActivity();
            Attachment attachment = new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = new AdaptiveCard("1.0")
                {
                    Body = new List<AdaptiveElement>()
                    {
                        new AdaptiveTextBlock() { Text = "FormField1 value was:", Size = AdaptiveTextSize.Large },
                        new AdaptiveTextBlock() { Text = eventData.Name }
                    },
                    Height = AdaptiveHeight.Auto,
                    Actions = new List<AdaptiveAction>()
                    {
                        new AdaptiveSubmitAction
                        {
                            Type = AdaptiveSubmitAction.TypeName,
                            Title = "Submit",
                            Data = new JObject { { "submitLocation", "messagingExtensionFetchTask" } },
                        }
                    }
                }
            };
            responseActivity.Attachments.Add(attachment);

            await turnContext.SendActivityAsync(responseActivity);


            return new MessagingExtensionActionResponse();
            //return response;
        }

        private MessagingExtensionActionResponse ShareMessageCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            // The user has chosen to share a message by choosing the 'Share Message' context menu command.
            var heroCard = new HeroCard
            {
                Title = $"{action.MessagePayload.From?.User?.DisplayName} orignally sent this message:",
                Text = action.MessagePayload.Body.Content,
            };

            if (action.MessagePayload.Attachments != null && action.MessagePayload.Attachments.Count > 0)
            {
                // This sample does not add the MessagePayload Attachments.  This is left as an
                // exercise for the user.
                heroCard.Subtitle = $"({action.MessagePayload.Attachments.Count} Attachments not included)";
            }
            
            // This Messaging Extension example allows the user to check a box to include an image with the
            // shared message.  This demonstrates sending custom parameters along with the message payload.
            var includeImage = ((JObject)action.Data)["includeImage"]?.ToString();
            if (!string.IsNullOrEmpty(includeImage) && bool.TrueString == includeImage)
            {
                heroCard.Images = new List<CardImage>
                {
                    new CardImage { Url = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU" },
                };
            }

            return new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "result",
                    AttachmentLayout = "list",
                    Attachments = new List<MessagingExtensionAttachment>()
                    {
                        new MessagingExtensionAttachment
                        {
                            Content = heroCard,
                            ContentType = HeroCard.ContentType,
                            Preview = heroCard.ToAttachment(),
                        },
                    },
                },
            };
        }

        private class Event
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDateTiem { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public string Location { get; set; }
            public int MinCapacity { get; set; }
            public int MaxCapacity { get; set; }
            public string PublishedChannelId { get; set; }
            public List<EventResponse> Responses { get; set; }
        }

        private class EventResponse
        {
            public string Id { get; set; }
            public Participant ResponsedBy { get; set; }
            public int Response { get; set; }
            public DateTime RespondeDateTime { get; set; }
        }

        private class Participant
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
