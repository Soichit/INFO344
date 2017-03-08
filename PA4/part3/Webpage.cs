using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text;
using System.Security.Cryptography;


namespace WebRole1
{
    public class Webpage : TableEntity
    {
        public string url { get; set; }
        public string title { get; set; }
        public string word { get; set; }
        public string date { get; set; }
        public string body { get; set; }
        public string imgUrl { get; set; }

        public Webpage() { }

        public Webpage(string url, string title, string word, string date, string body, string imgUrl)
        {
            this.PartitionKey = word;
            this.RowKey = GetHashString(url);
            this.url = url;
            this.title = title;
            this.word = word;
            this.date = date;
            this.body = body;
            this.imgUrl = imgUrl;
        }

        public static byte[] GetHash(string inputString)
        {
            HashAlgorithm algorithm = MD5.Create();  //or use SHA1.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string GetHashString(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }
    }
}