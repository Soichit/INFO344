using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Azure; // Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Blob; // Namespace for Blob storage types


namespace PA2
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]S
    public class WebService1 : System.Web.Services.WebService
    {
        public Trie myTrie = new Trie();


        [WebMethod]
        public void retrieveDump()
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
            CloudConfigurationManager.GetSetting("StorageConnectionString"));

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("wiki-dump");

            if (container.Exists())
            {
                foreach (IListBlobItem item in container.ListBlobs(null, false))
                {
                    if (item.GetType() == typeof(CloudBlockBlob))
                    {
                        CloudBlockBlob blob = (CloudBlockBlob)item;
                        // Retrieve reference to my blob
                        // CloudBlockBlob blockBlob = container.GetBlockBlobReference("hello-blob.txt");
                        //using (var fileStream = System.IO.File.OpenWrite(@"path\"))
                        //{
                        //    blob.DownloadToStream(fileStream);
                        //}
                        using (var memoryStream = new MemoryStream())
                        {
                            //memorystream to string array
                            //download to azure locally

                            blob.DownloadToStream(memoryStream);
                        }
                    }
                }
            }
        }


        [WebMethod]
        public List<String> AddTitles()
        {
            String text = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            /// insert into trie
            using (StreamReader sr = new StreamReader("/Users/iGuest/documents/abc.txt"))
            {
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    myTrie.insert(line);
                    //s += line + "\n";
                }
            }



            ///search
            List<String> result = myTrie.search("ab");
            //List<String> result = new List<string>(new string[] { "element1", "element2", "element3" });
            return result;
        }
    }
}
