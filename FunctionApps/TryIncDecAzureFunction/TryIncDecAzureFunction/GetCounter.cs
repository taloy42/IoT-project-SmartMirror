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

namespace TryIncDecAzureFunction
{
    public static class GetCounter
    {
        [FunctionName("GetCounter")]
        public static async Task<CounterEntity> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post","options", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string accountName = "tryincdecstorageaccount";
            string accountKey = "sdCItKzHCN552PNs8DE5Wp+xANGNehrMgHbXBb1TIFlsl2VCLntT13Dl0/RQ5ZNDt/R2btqrBjS2+ASt5ES2PA==";


            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");


            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable counterTable = tableClient.GetTableReference("TryIncDecTable");


            /****************/
            //TableOperation updateOperation = TableOperation.Replace(counterEntity);
            //await counterTable.ExecuteAsync(updateOperation);
            ///****************/

            /****************/
            TableOperation retrieveOperation = TableOperation.Retrieve<CounterEntity>("0", "0");

            TableResult result = await counterTable.ExecuteAsync(retrieveOperation);
            CounterEntity res = result?.Result as CounterEntity ?? null;


            /****************/



            string responseMessage = res.Counter.ToString();

            //return new OkObjectResult(responseMessage);
            return res;
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
