using System;

namespace farmamest.Models
{
    public class WebhookNotification
    {
        public string Id { get; set; }
        public string EventType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public dynamic Data { get; set; }
    }
}