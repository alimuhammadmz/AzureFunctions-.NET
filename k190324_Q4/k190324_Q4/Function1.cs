using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Configuration;
using System.Drawing;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace k190324_Q4
{
    public static class Function1
    {
        [FunctionName("myAzureFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            string responseMessage = "";

            if (string.IsNullOrEmpty(name))
                responseMessage = "Invalid Script name!";
            else
            {
                int fg = 0;
                string root = Environment.GetEnvironmentVariable("root");
                string[] dirs = Directory.GetDirectories(root+"/Output/", "*", SearchOption.TopDirectoryOnly);

                for (int i = 0; i < dirs.Length; i++)
                {
                    if (File.Exists(dirs[i] + "/" + name + ".json"))
                    {
                        using (StreamReader file = File.OpenText(dirs[i] + "/" + name + ".json"))
                        {
                            JsonTextReader reader = new JsonTextReader(file);
                            JsonSerializer serial = new JsonSerializer();
                            object parsedData = serial.Deserialize(reader);
                            responseMessage = parsedData.ToString();
                        }
                        fg = 1;
                        break;
                    }
                }
                if(fg == 0)
                    responseMessage = "Invalid Script name!";
            }
            return new OkObjectResult(responseMessage);
        }
    }
}
