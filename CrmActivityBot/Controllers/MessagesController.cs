using CrmActivityBot.Models;
using CrmActivityBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace CrmActivityBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<AppointmentReport> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(AppointmentReport.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var stateClient = activity.GetStateClient(ConfigurationManager.AppSettings["MicrosoftAppId"], ConfigurationManager.AppSettings["MicrosoftAppSecret"]);
                var userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
                var privateData = await stateClient.BotState.GetPrivateConversationDataAsync(activity.ChannelId, activity.Conversation.Id, activity.From.Id);

                // CRMにアクセスしてそのユーザーの本日の予定データを取得します。
                // 本来、この処理は一連のFormFlowの会話が完了する間には一度だけ実行することが望ましい
                CrmService service = new Services.CrmService();
                CrmUser user = await service.GetCrmUser(activity.From.Name);
                CrmService.CallerId = user.Id.ToString();
                AppointmentReport.CrmUserName = user.FullName;
                var appointments = await service.GetCRMAppointments();

                if (appointments.Records.Count() != 0)
                    await Conversation.SendAsync(activity, MakeRootDialog);
                else
                    connector.Conversations.SendToConversation(activity.CreateReply("本日の未報告の訪問はありません。"));
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}