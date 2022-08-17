

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace TryIncDecAzureFunction
{
    public static class UpdateCounter
    {
        [FunctionName("UpdateCounter")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post","options", Route = null)] HttpRequest req, 
            [SignalR(HubName = "TryIncDecSingalRHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {

            string responseMessage;
            log.LogInformation("C# HTTP trigger function processed a request.");


            string cntr_str = req.Query["counter"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            cntr_str = cntr_str ?? data?.counter;

            int counter;
            bool isNumeric = int.TryParse(cntr_str, out counter);

            if (!isNumeric)
            {
                responseMessage = $"value of counter is not integer";
                return new OkObjectResult(responseMessage);
            }
            CounterEntity counterEntity = new CounterEntity("0", "0", counter);

            counterEntity.ETag = "*";

            string accountName = "tryincdecstorageaccount";
            string accountKey = "sdCItKzHCN552PNs8DE5Wp+xANGNehrMgHbXBb1TIFlsl2VCLntT13Dl0/RQ5ZNDt/R2btqrBjS2+ASt5ES2PA==";


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");


            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable counterTable = tableClient.GetTableReference("TryIncDecTable");


            /****************/
            TableOperation updateOperation = TableOperation.Replace(counterEntity);
            await counterTable.ExecuteAsync(updateOperation);
            /****************/


            //Relay message to all clients via SignalR

            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "IncDecDevices",
                    Arguments = new[] { $"Counter value is: {counterEntity.Counter}", counterEntity.Counter.ToString()}
                });

            responseMessage = $"new counter value is {counter}";

            return new OkObjectResult(responseMessage);
        }


        public class CounterEntity : TableEntity
        {
            public CounterEntity() { }

            public CounterEntity(string partition, string row, int counter)
            {
                PartitionKey = partition;
                RowKey = row;
                Counter = counter;
            }
            public int Counter { get; set; }
        }
    }
}
