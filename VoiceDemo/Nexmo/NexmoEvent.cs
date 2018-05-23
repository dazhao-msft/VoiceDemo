using System;
using System.Runtime.Serialization;

namespace VoiceDemo.Nexmo
{
    [DataContract]
    public class NexmoEvent
    {
        [DataMember(Name = "conversation_uuid")]
        public string ConversationUuid { get; set; }

        [DataMember(Name = "to")]
        public string To { get; set; }

        [DataMember(Name = "from")]
        public string From { get; set; }

        [DataMember(Name = "direction")]
        public string Direction { get; set; }

        [DataMember(Name = "recording_url")]
        public string RecordingUrl { get; set; }

        [DataMember(Name = "rate")]
        public decimal? Rate { get; set; }

        [DataMember(Name = "start_time")]
        public DateTimeOffset? StartTime { get; set; }

        [DataMember(Name = "network")]
        public string Network { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "price")]
        public decimal? Price { get; set; }

        [DataMember(Name = "duration")]
        public int? Duration { get; set; }

        [DataMember(Name = "end_time")]
        public DateTimeOffset? EndTime { get; set; }

        [DataMember(Name = "timestamp")]
        public DateTimeOffset? Timestamp { get; set; }

        [DataMember(Name = "uuid")]
        public string Uuid { get; set; }
    }
}
