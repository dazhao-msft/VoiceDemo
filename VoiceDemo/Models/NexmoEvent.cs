using System;
using System.Runtime.Serialization;

namespace VoiceDemo.Models
{
    [DataContract]
    public class NexmoEvent
    {
        [DataMember(Name = "start_time")]
        public DateTimeOffset? StartTime { get; set; }

        [DataMember(Name = "end_time")]
        public DateTimeOffset? EndTime { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

        [DataMember(Name = "uuid")]
        public string Uuid { get; set; }

        [DataMember(Name = "conversation_uuid")]
        public string ConversationUuid { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "direction")]
        public string Direction { get; set; }

        [DataMember(Name = "network")]
        public string Network { get; set; }

        [DataMember(Name = "price")]
        public string Price { get; set; }

        [DataMember(Name = "rate")]
        public string Rate { get; set; }

        [DataMember(Name = "duration")]
        public string Duration { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTimeOffset? Timestamp { get; set; }
    }
}
