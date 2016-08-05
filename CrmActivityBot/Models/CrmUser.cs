using Newtonsoft.Json;
using System;

namespace CrmActivityBot.Models
{
    public class CrmUser
    {
        [JsonProperty("systemuserid")]
        public Guid Id { get; set; }
        [JsonProperty("fullname")]
        public string FullName { get; set; }
    }
}