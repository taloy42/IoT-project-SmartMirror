using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace TryIncDecAzureFunction
{
    public static class HttpExample
    {
        [FunctionName("HttpExample")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name;
            name = req.Query["name"];
            //var Qname = req.QueryString;

            var f = req.Form;
            StringValues stringValues;
            var z = f.TryGetValue("name", out stringValues);
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();


            //name = "menashnesh";
            var fs = f.Files;
            var first = fs[0];
            using (Stream stream = first.OpenReadStream())
            {
                var bitmap = Image.FromStream(stream);

                bitmap.Save(name + ".png");

                log.LogInformation(bitmap.ToString());

            }
            log.LogInformation(req.Form.Files[0].Name);

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }

        [FunctionName("ExampleURL")]
        public static async Task<IActionResult> Run2(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request. BLOB===========   ");



            return new OkObjectResult("hi");
        }

    }
}
