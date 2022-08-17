using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;


using Microsoft.Azure.WebJobs.Extensions.SignalRService;


using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace TryIncDecAzureFunction
{
    public class MessageReceiver
    {
        [FunctionName("MessageReceiver")]
        public async Task Run([EventHubTrigger("tryincdeceventhub", Connection = "DefaultSettings")] string[] messages,
            [SignalR(HubName = "TryIncDecSingalRHub")] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            var exceptions = new List<Exception>();

            foreach (string message in messages)
            {
                try
                {
                   
                    //initialization

                    string accountName = "tryincdecstorageaccount";
                    string accountKey = "sdCItKzHCN552PNs8DE5Wp+xANGNehrMgHbXBb1TIFlsl2VCLntT13Dl0/RQ5ZNDt/R2btqrBjS2+ASt5ES2PA==";


                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                        $"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={accountKey}");


                    CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                    CloudTable counterTable = tableClient.GetTableReference("TryIncDecTable");


                    //get current counter
                    TableOperation retrieveOperation = TableOperation.Retrieve<CounterEntity>("0", "0");

                    TableResult result = await counterTable.ExecuteAsync(retrieveOperation);
                    CounterEntity res = result?.Result as CounterEntity ?? null;

                    int counterValue = res.Counter;


                    //update counter based on message
                    counterValue++;


                    //create new counter
                    string partitionKey = "0";
                    string rowKey = "0";
                    CounterEntity counterEntity = new CounterEntity(partitionKey, rowKey, counterValue);
                    counterEntity.ETag = "*";


                    //update counter in the storage table
                    TableOperation updateOperation = TableOperation.Replace(counterEntity);
                    await counterTable.ExecuteAsync(updateOperation);

                    //Relay message to all clients via SignalR

                    await signalRMessages.AddAsync(
                    new SignalRMessage
                    {
                        Target = "IncDecDevices",
                        Arguments = new[] { $"Counter value is: {counterEntity.Counter}", counterEntity.Counter.ToString() }
                    });


                    /**zehu**/
                }
                catch (Exception e)
                {
                    // We need to keep processing the rest of the batch - capture this exception and continue.
                    // Also, consider capturing details of the message that failed processing so it can be processed again later.
                    exceptions.Add(e);
                }
            }

            // Once processing of the batch is complete, if any messages in the batch failed processing throw an exception so that there is a record of the failure.

            if (exceptions.Count > 1)
                throw new AggregateException(exceptions);

            if (exceptions.Count == 1)
                throw exceptions.Single();
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
