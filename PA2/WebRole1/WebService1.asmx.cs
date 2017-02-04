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
    public class WebService1 : System.Web.Services.WebService
    {

        private static Trie trie = new Trie();
        private string filePath = Path.GetTempPath() + "\\wiki.txt";
        //private string filePath = "/Users/iGuest/documents/wiki-output.txt";
        //private string filePath = "/Users/iGuest/documents/abc.txt";
        private int memoryCap = 20; // change memory to 20

        [WebMethod]
        public String downloadWiki()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wiki-blob");

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
                       
                        if (memory <= memoryCap)
                        {
                            break;
                        }
                    }
                    string line = sr.ReadLine().Trim().ToLower();
                    trie.insert(line);
                    titleCounter++;
                }
            }
            return "Trie built!";
        }

        [WebMethod]
        public String saveUserSearch(String input)
        {
            String check = trie.addUserSearch(input);
            if (check == null)
            {
                return "Your input is misspelled";
            }
            return "Search query saved!";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public String SearchTrie(String input)
        {
            Dictionary<String, int> result = trie.search(input);
            //List<String> result = trie.search(input);
            //return result;

            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(result);
        }
    }
}
