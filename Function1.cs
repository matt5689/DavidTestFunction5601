using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading;

namespace DavidTestFunction5601
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            //========================

            for (int i = 0; i < Convert.ToInt32(Environment.GetEnvironmentVariable("LoopCount")); i++)
            {
                var child = Task.Factory.StartNew(() => {
                    Console.WriteLine("Nested task starting.");
                    if(Environment.GetEnvironmentVariable("useSpinWait").ToLower() == "true")
                    {
                        Thread.SpinWait(Convert.ToInt32(Environment.GetEnvironmentVariable("SleepTime")));
                    }
                    else
                    {
                        Thread.Sleep(Convert.ToInt32(Environment.GetEnvironmentVariable("SleepTime")));
                    }
                    Console.WriteLine("Nested task completing.");
                });
            }       //========================

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
    }
}
