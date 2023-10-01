using System.Collections.Generic;
using Newtonsoft.Json;

namespace ChatBot.FromLine.Model
{
    public class LineRequestModel
    {
        [JsonProperty("destination")]
        public string Destination { get; set; }
        [JsonProperty("events")]
        public List<LineEventModel> Evnet { get; set; }
    }

    public class LineEventModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("message")]
        public LineMessageModel Message { get; set; }
        [JsonProperty("webhookEvenId")]
        public string WebhookEventId { get; set; }
        [JsonProperty("deliveryContext")]
        public LineDeliveryContextModel DeliveryContext { get; set; }
        [JsonProperty("timestamp")]
        public string TimeStamp { get; set; }
        [JsonProperty("source")]
        public LineSourceModel Souce { get; set; }
        [JsonProperty("replyToken")]
        public string ReplyToken { get; set; }
        [JsonProperty("mode")]
        public string Mode { get; set; }
    }

    public class LineMessageModel
    {
        [JsonProperty("type")]
        public string  Type { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
    }

    public class LineDeliveryContextModel
    {
        [JsonProperty("isRedelivery")]
        public bool IsRedelivery { get; set; }
    }

    public class LineSourceModel
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("userId")]
        public string UserId { get; set; }
    }
}