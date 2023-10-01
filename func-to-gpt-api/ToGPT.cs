using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ChatBot.ToGPT.Utility;

namespace ChatBot.ToGPT
{
    public static class ToGPT
    {
        [FunctionName("ToGPT")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string message = req.Query["text"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            message = message ?? data?.message;

            if (string.IsNullOrWhiteSpace(message))
            {
                return new BadRequestObjectResult("Message is null");
            }

            var resultMessage = await GPTUtility.ToGPT(log, message);

            return string.IsNullOrWhiteSpace(resultMessage) ? new NotFoundObjectResult("") : new OkObjectResult(resultMessage);
        }
    }
}
