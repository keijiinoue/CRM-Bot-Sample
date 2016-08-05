using CrmActivityBot.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.FormFlow.Advanced;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrmActivityBot.Models
{
    /// <summary>
    /// オプションセット型のカスタムフィールド「次のアクション」のラベルと値
    /// </summary>
    public enum NextAction : int
    {
        ヒアリング = 100000000, 提案 = 100000001, 見積もり = 100000002, 交渉 = 100000003, 契約 = 100000004
    }

    [Serializable]
    public class AppointmentReport
    {
        static private Appointments appointments;
        static private Appointment currentAppointment;
        static public string CrmUserName;
        
        public static IForm<AppointmentReport> BuildForm()
        {
            FormBuilder<AppointmentReport> diaryForm = new FormBuilder<AppointmentReport>();
            return diaryForm
                .Message($"{CrmUserName}さん、お疲れさまです。訪問に関する活動報告を残します。")
                .Field(new FieldReflector<AppointmentReport>(nameof(AppointmentName))
                        .SetType(null)
                        .SetActive((state) => true)
                        .SetDefine(SetAppointments)
                        .SetPrompt(new PromptAttribute("訪問を選択してください {||}")))
                .Field(nameof(Description), new PromptAttribute("内容を入力してください"))
                .Field(nameof(NextAction), new PromptAttribute("次のアクションを選択してください {||}"))
                .OnCompletion(OnComplete)
                .Build();
        }
        /// <summary>
        /// CRMの予定レコードを取得し、選択肢としてセットする。
        /// </summary>
        /// <param name="state"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        private static async Task<bool> SetAppointments(AppointmentReport state, Field<AppointmentReport> field)
        {
            if (appointments == null)
            {
                CrmService service = new Services.CrmService();
                appointments = await service.GetCRMAppointments();
            }

            foreach (Appointment record in appointments.Records)
            {                
                    field
                        .AddDescription(record.Subject, record.Subject)
                        .AddTerms(record.Subject, record.Subject);                
            }

            return true;
        }

        private static async Task OnComplete(IDialogContext context, AppointmentReport state)
        {
            // CRMの予定レコードを更新する。
            CrmService service = new Services.CrmService();
            currentAppointment = appointments.Records.Where(x => x.Subject == state.AppointmentName).First();
            currentAppointment.Description = state.Description;
            currentAppointment.NextAction = (int)state.NextAction;
            await service.Update("appointments", currentAppointment.Id, currentAppointment);

            string message = (appointments.Records.Count() == 1) ? 
                "本日の未報告の訪問はありません。" : 
                $"活動報告を Dynamics CRM に登録しました。本日はもう {appointments.Records.Count() - 1} 件の訪問が残っています。頑張ってください！！";
            appointments = null;
            await context.PostAsync(message);
        }
        
        [Template(TemplateUsage.NoPreference, "None")]
        public string AppointmentName;
        public string Description;
        public NextAction NextAction;
    }
}