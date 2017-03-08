using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Script.Serialization;
using WebRole1;
using Microsoft.WindowsAzure.Storage.Table;


namespace WebRole1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [ScriptService]
    public class QuerySuggest : System.Web.Services.WebService
    {

        private static Trie trie;
        private string filePath = Path.GetTempPath() + "\\wiki.txt";
        //private string filePath = "/Users/iGuest/documents/wiki-output.txt";
        //private string filePath = "/Users/iGuest/documents/abc.txt";
        private int memoryCap = 20;
        private static CloudTable statsTable;
        private static Stats stats;

        [WebMethod]
        public String downloadWiki()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wiki-blob");

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            statsTable = tableClient.GetTableReference("stats");
            statsTable.CreateIfNotExists();

            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        ///Retrieve reference to my blob
                        CloudBlockBlob blockBlob = container.GetBlockBlobReference("ProcessedWikiDump.txt");
                        using (var fileStream = System.IO.File.OpenWrite(filePath))
                        {
                           blob.DownloadToStream(fileStream);
                        }
                    }
                }
            }
            return "Wiki downloaded!";
        }

        [WebMethod]
        public String buildTrie()
        {
            if (!File.Exists(filePath))
            {
                downloadWiki();
                return "Wiki file being downloaded...";
            } else
            {
                stats = new Stats();
                trie = new Trie();
                int titleCounter = 1;
                PerformanceCounter theMemCounter = new PerformanceCounter("Memory", "Available MBytes");

                using (StreamReader sr = new StreamReader(filePath))
                {
                    while (!sr.EndOfStream)
                    {
                        // check for memeory every 1000 lines
                        if (titleCounter % 1000 == 0)
                        {
                            float memory = theMemCounter.NextValue();
                            try
                            {
                                TableOperation insertOperation = TableOperation.InsertOrReplace(stats);
                                statsTable.ExecuteAsync(insertOperation);
                            }
                            catch (Exception e)
                            {
                                Trace.TraceInformation("Error: " + e.Message);
                            }
                            if (memory <= memoryCap)
                            {
                                break;
                            }
                        }
                        string line = sr.ReadLine().Trim().ToLower();
                        trie.insert(line);
                        titleCounter++;
                        stats.updateWiki(line);
                    }
                }
                return "Trie built!";
            }
        }

        //[WebMethod]
        //public String saveUserSearch(String input)
        //{
        //    String check = trie.addUserSearch(input);
        //    if (check == null)
        //    {
        //        return "Your input is misspelled";
        //    }
        //    return "Search query saved!";
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String SearchTrie(String input)
        {
            if (trie == null)
            {
                buildTrie();
                return null;
            } else
            {
                Dictionary<String, int> result = trie.search(input);
                //List<String> result = trie.search(input);
                //return result;
                JavaScriptSerializer jss = new JavaScriptSerializer();
                return jss.Serialize(result);
            }
        }
    }
}
