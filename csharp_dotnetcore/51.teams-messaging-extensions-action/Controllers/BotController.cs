// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;
using WCB.TeamMeet.Domain;

namespace TeamsMessagingExtensionsAction.Controllers
{
    // This ASP Controller is created to handle a request. Dependency Injection will provide the Adapter and IBot
    // implementation at runtime. Multiple different IBot implementations running at different endpoints can be
    // achieved by specifying a more specific type for the bot constructor argument.
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotFrameworkHttpAdapter Adapter;
        private readonly IBot Bot;
        private readonly DialogSet _dialogs;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            Adapter = adapter;
            Bot = bot;
            _dialogs = new DialogSet();
        }

        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await Adapter.ProcessAsync(Request, Response, Bot);
        }

        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            DialogContext dc = null;
            string adaptiveCard = string.Empty;

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.ConversationUpdate:
                    foreach (var member in turnContext.Activity.MembersAdded)
                    {
                        //if (member.Id != turnContext.Activity.Recipient.Id)
                        //{
                        //    adaptiveCard = File.ReadAllText(@".\wwwroot\ColorSelector.json");
                        //    var reply = turnContext.Activity.CreateReply();
                        //    reply.Attachments = new List<Attachment>()
                        //    {
                        //        new Attachment()
                        //        {
                        //            ContentType = "application/vnd.microsoft.card.adaptive",
                        //            Content = JsonConvert.DeserializeObject(adaptiveCard)
                        //        }
                        //    };

                        //    await turnContext.SendActivityAsync(reply, cancellationToken: cancellationToken);
                        //}
                    }

                    break;

                case ActivityTypes.Message:
                    try
                    {
                        var token = JToken.Parse(turnContext.Activity.ChannelData.ToString());
                        string selectedcolor = string.Empty;
                        var response = new EventResponse();
                        if (Convert.ToBoolean(token["postback"].Value<string>()))
                        {
                            JToken commandToken = JToken.Parse(turnContext.Activity.Value.ToString());
                            string action = commandToken["action"].Value<string>();
                            //if (command.ToLowerInvariant() == "colorselector")
                            //{
                            //    selectedcolor = commandToken["choiceset"].Value<string>();
                            //}

                            response.EventId = Guid.NewGuid().ToString();
                            response.ResponseContent = action == "true" ? 1 : 0;
                            response.ResponseUserId = turnContext.Activity.From.Id;
                            response.ResponseUsesrFirstName = turnContext.Activity.From.Name;
                        }

                        var yes = $"{turnContext.Activity.From.Name} is attending";
                        var no = $"{turnContext.Activity.From.Name} is NOT attending";

                        await turnContext.SendActivityAsync(response.ResponseContent == 1 ? yes : no,
                            cancellationToken: cancellationToken);
                    }
                    catch
                    {
                        await turnContext.SendActivityAsync("something went wrong",
                            cancellationToken: cancellationToken);
                    }

                    break;

            }

        }
    }
}
