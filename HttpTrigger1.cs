using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using Renci.SshNet;

namespace Company.Function
{
    public static class HttpTrigger1
    {
        [FunctionName("HttpTrigger1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            log.LogInformation("buddhi log");

            var client = new SftpClient("sftp-gears-prod.shift-technology.com", "direct-asia-insurance-download", "qmjFAl5bPXJ41plVd6yv");
            var files = new List<String>();
            client.Connect();
            ListDirectory(client, ".", ref files);
            client.Disconnect();
            // files.Dump();

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }


        static void ListDirectory(SftpClient client, String dirName, ref List<String> files)
        {
            foreach (var entry in client.ListDirectory(dirName))
            {
                if (entry.IsDirectory)
                {
                    Console.WriteLine("directory: "+entry.FullName);
                    ListDirectory(client, entry.FullName, ref files);
                }
                else
                {
                    files.Add(entry.FullName);
                    Console.WriteLine("file: "+entry.FullName);
                }
            }
        }

    }
}
