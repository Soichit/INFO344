using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;



namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]

    [ScriptService]
    public class Admin : System.Web.Services.WebService
    {
        private static CloudQueue stateQueue;
        private static CloudQueue crawlQueue;
        private static CloudTable statsTable;
        private static CloudTable errorsTable;
        private static CloudTable resultsTable;
        private static Dictionary<string, List<PageCount>> cache;

        public Admin()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            statsTable = tableClient.GetTableReference("stats");
            statsTable.CreateIfNotExists();
            errorsTable = tableClient.GetTableReference("errors");
            errorsTable.CreateIfNotExists();
            resultsTable = tableClient.GetTableReference("urls");
            resultsTable.CreateIfNotExists();

            stateQueue = queueClient.GetQueueReference("state");
            stateQueue.CreateIfNotExists();
            crawlQueue = queueClient.GetQueueReference("crawl");
            crawlQueue.CreateIfNotExists();
            cache = new Dictionary<string, List<PageCount>>();
        }

        [WebMethod]
        public string StartCrawling()
        {
            CloudQueueMessage message = new CloudQueueMessage("start");
            stateQueue.AddMessage(message);
            return "done";
        }

        [WebMethod]
        public string StopCrawling()
        {
            CloudQueueMessage message = new CloudQueueMessage("stop");
            stateQueue.AddMessage(message);
            return "done";
        }

        [WebMethod]
        public string ClearIndex()
        {
            CloudQueueMessage message = new CloudQueueMessage("clear");
            stateQueue.AddMessage(message);
            return "done";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string AddtoCrawler(string input)
        {
            List<String> crawledList = new List<string>();

            //gets statsTable.crawled, then check for duplicates and add to stats table
            TableOperation retrieveOperation = TableOperation.Retrieve<Stats>("PartionKey", "RowKey");
            TableResult retrievedResult = statsTable.Execute(retrieveOperation);
            if (retrievedResult.Result != null)
            {
                string result = ((Stats)retrievedResult.Result).crawled;
                if (result != "")
                {
                    crawledList = result.Split(',').ToList();
                    crawledList.Remove("");
                }
            }

            input = input.ToLower();
            if (input.Contains("cnn"))
            {
                if (crawledList.Contains("http://www.cnn.com"))
                {
                    return "Duplicate";
                }
                else
                {
                    CloudQueueMessage message = new CloudQueueMessage("http://www.cnn.com");
                    crawlQueue.AddMessage(message);
                    crawledList.Add("http://www.cnn.com");
                }
            }
            else if (input.Contains("bleacher"))
            {
                if (crawledList.Contains("http://bleacherreport.com"))
                {
                    return "Duplicate";
                }
                else
                {
                    CloudQueueMessage message = new CloudQueueMessage("http://bleacherreport.com");
                    crawlQueue.AddMessage(message);
                    crawledList.Add("http://bleacherreport.com");
                }
            }
            else
            {
                return "Can't";
            }
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(crawledList);
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getResults(string input)
        {
            try
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                if (cache.Count >= 100)
                {
                    cache.Clear();
                }
                if (cache.ContainsKey(input))
                {
                    return jss.Serialize(cache[input]);
                }
                else
                {
                    List<string> inputWords = input.Split(' ').ToList();

                    Dictionary<string, PageCount> results = new Dictionary<string, PageCount>();
                    foreach (string word in inputWords)
                    {
                        TableQuery<Webpage> rangeQuery = new TableQuery<Webpage>().Where(
                        TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, word));
                        var resultsQuery = resultsTable.ExecuteQuery(rangeQuery).ToList();
                        foreach (Webpage page in resultsQuery)
                        {
                            if (results.ContainsKey(page.url))
                            {
                                //results[page.url].increment();
                                results[page.url].count++;
                            }
                            else
                            {
                                results.Add(page.url, new PageCount(page));
                            }
                        }
                    }

                    // ranking system based off count and date
                    List<PageCount> filteredResults = results.OrderByDescending(x => x.Value.count)
                    .ThenByDescending(x => x.Value.webpage.date)
                    .Take(15)
                    .Select(x => x.Value)
                    .ToList();

                    cache.Add(input, filteredResults);
                    return jss.Serialize(filteredResults);
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getStats()
        {
            try
            {
                var result = statsTable.ExecuteQuery(new TableQuery<Stats>()).ToList();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(result);
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }



        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string getErrors()
        {
            try
            {
                var query = errorsTable.ExecuteQuery(new TableQuery<Errors>()).ToList();
                List<Errors> result = new List<Errors>();
                JavaScriptSerializer jss = new JavaScriptSerializer();
                foreach (Errors error in query)
                {
                    result.Add(error);
                    if (result.Count >= 10)
                    {
                        return jss.Serialize(result);
                    }
                }
                return jss.Serialize(result);
            }
            catch (Exception e)
            {
                return e.ToString();
            } 
        }
    }
}
