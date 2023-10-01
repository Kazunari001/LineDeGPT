using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChatBot.FromLine.Model;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ChatBot.FromLine
{
    public static class FromLine
    {
        private static HttpClient client = new HttpClient();
        [FunctionName("FromLine")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                LineRequestModel data = JsonConvert.DeserializeObject<LineRequestModel>(requestBody);

                string text = await ToGPTServer.APIServerRequest(log, data.Evnet[0].Message.Text);

                // Build Response Message
                var responseBody = new {
                    to = data.Evnet[0].Souce.UserId,
                    messages = new List<Message>()
                    {
                        new Message()
                        {
                            text = string.IsNullOrWhiteSpace(text) ? "ChatGPTからの情報の取得に失敗しました。\n再度メッセージを送信してください。" : text,
                            type = "text"
                        }
                    }
                };
                var responseContent = JsonConvert.SerializeObject(responseBody);
                log.LogInformation(responseContent);

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(Environment.GetEnvironmentVariable("LineAPIUri")),
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                };
                request.Headers.Add("Authorization", $"Bearer {Environment.GetEnvironmentVariable("LineAPIKey")}");

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    var errorStatusCode = response.StatusCode;
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    log.LogError($"HTTP Error {errorStatusCode}: {errorMessage}");
                }
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
            }

            return new OkObjectResult("");
        }
    }

    public class Message
    {
        public string text { get; set; }
        public string type { get; set; }
    }
}
