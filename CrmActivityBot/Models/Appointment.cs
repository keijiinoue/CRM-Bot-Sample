using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CrmActivityBot.Models
{
    public class Appointments
    {
        [JsonProperty("value")]
        public List<Appointment> Records { get; set; }
    }
    public class Appointment
    {
        [JsonProperty("activityid")]
        public Guid Id { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        /// <summary>
        /// オプションセット型のカスタムフィールド「次のアクション」
        /// </summary>
        [JsonProperty("new_nextaction")]
        public int NextAction { get; set; }
    }
}