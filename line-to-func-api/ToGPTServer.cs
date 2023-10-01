using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ChatBot.FromLine
{
    public class ToGPTServer
    {
        /// <summary>
        /// GPTサーバに向けてAPIリクエストを送る
        /// </summary>
        /// <param name="log">ロガー</param>
        /// <param name="text">リクエストメッセージ</param>
        /// <return>GPTサーバからのメッセージ</return>
        public static async Task<string> APIServerRequest(ILogger log, string text)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage();

                var requestBody = JsonConvert.SerializeObject(text);
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(Environment.GetEnvironmentVariable("APIServerUrl"));
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                var message = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    var errorStatusCode = response.StatusCode;
                    log.LogError($"HTTP Error {errorStatusCode}: {message}");
                    return string.Empty;
                }

                return message;
            }
            catch (Exception e)
            {
                log.LogError(e.Message);
                return string.Empty;
            }
        }
    }
}