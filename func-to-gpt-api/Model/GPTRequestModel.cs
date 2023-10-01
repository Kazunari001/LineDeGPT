using System.Collections.Generic;

namespace ChatBot.ToGPT.Model.Request
{
    public class GPTRequestModel
    {
        public string model { get; set; }
        public List<Message> messages { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}