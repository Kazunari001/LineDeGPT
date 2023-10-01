using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChatBot.ToGPT.Model.Request;
using ChatBot.ToGPT.Model.Response;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatBot.ToGPT.Utility
{
    public class GPTUtility
    {
        private static readonly string model = Environment.GetEnvironmentVariable("GPTModel");
        private static readonly Uri url = new(Environment.GetEnvironmentVariable("GPTURL"));
        private static readonly string gptKey = Environment.GetEnvironmentVariable("GPTAPIKey");
        private static HttpClient client = new HttpClient();
        private static HttpRequestMessage request = new HttpRequestMessage();
        public static async Task<string> ToGPT(ILogger log, string message)
        {
            var body = new GPTRequestModel{
                model = model,
                messages = new List<Model.Request.Message>
                {
                    new() {
                        role = "user",
                        content = message
                    }
                }
            };
            var requestBody = JsonConvert.SerializeObject(body);
            request.Method = HttpMethod.Post;
            request.RequestUri = url;
            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {gptKey}");

            // Register ServiceBus
            await ServiceBusUtility.SendServiceBusQueue(log, message);

            log.LogInformation($"to gpt \n[Method]:Post\n[Content]:{requestBody}");
            HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
            string responseMessage = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var errorStatusCode = response.StatusCode;
                log.LogError($"HTTP Error {errorStatusCode}:{responseMessage}");
                return string.Empty;
            }

            // Register ServiceBus
            await ServiceBusUtility.SendServiceBusQueue(log, responseMessage);

            var choiceMessage = JsonConvert.DeserializeObject<GPTResponseModel>(responseMessage);

            return choiceMessage.Choices[0].Message.Content;
        }

    }
}