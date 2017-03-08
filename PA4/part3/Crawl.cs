using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;

//using WorkerRole1;
//using System.Web;
using HtmlAgilityPack;
//using Microsoft.WindowsAzure.ServiceRuntime;


namespace WebRole1
{
    public class Crawl
    {
        // static variables??
        public CloudQueue htmlQueue { get; private set; }
        public List<String> xmlList { get; private set; }
        public List<String> robotXmlList { get; private set; }
        public CloudTable urlsTable { get; private set; }
        public CloudTable errorsTable { get; private set; }
        public string baseUrl { get; private set; }
        public DateTime cutOffDate { get; private set; }
        public HashSet<string> disallows { get; private set; }
        public HashSet<string> duplicates { get; private set; }

        public Crawl() {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                 CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            urlsTable = tableClient.GetTableReference("urls");
            urlsTable.CreateIfNotExistsAsync();
            errorsTable = tableClient.GetTableReference("errors");
            errorsTable.CreateIfNotExistsAsync();

            xmlList = new List<String>();
            robotXmlList = new List<String>();
            htmlQueue = queueClient.GetQueueReference("myhtml");
            htmlQueue.CreateIfNotExists();
            disallows = new HashSet<string>();
            duplicates = new HashSet<string>();
            cutOffDate = new DateTime(2017, 1, 1);
        }

        public void parseXML(string url)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(url);
                string tag = "";
                Boolean dateAllowed = true;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element: //tag types
                            tag = reader.Name;
                            break;
                        case XmlNodeType.Text: //text within tags
                            // if vs elseif
                            // for cases where lastmod tag doesn't exist
                            if (tag == "sitemap" || tag == "url")
                            {
                                dateAllowed = true;
                            }
                            if (tag == "lastmod")
                            {
                                if (baseUrl == "http://www.cnn.com")
                                {
                                    // checking date only for cnn
                                    string date = reader.Value.Substring(0, 10); //format: 2017-02-17 
                                    DateTime dateTime = Convert.ToDateTime(date);
                                    int compare = DateTime.Compare(dateTime, cutOffDate);
                                    if (compare >= 0)
                                    {
                                        dateAllowed = true;
                                    }
                                    else
                                    {
                                        dateAllowed = false;
                                    }
                                } else if (baseUrl == "http://bleacherreport.com")
                                {
                                    dateAllowed = true;
                                }
                            }
                            if (tag == "loc")
                            {
                                string link = reader.Value;

                                //check if the date is allowed and robot.txt link is allowed
                                if (dateAllowed && !disallows.Any(link.ToLower().StartsWith))
                                {
                                    if (link.Substring(link.Length - 4) == ".xml")
                                    {
                                        xmlList.Add(link);
                                    }
                                    else
                                    {
                                        CloudQueueMessage htmlLink = new CloudQueueMessage(reader.Value);
                                        //assuming the type is .html or .htm
                                        htmlQueue.AddMessage(htmlLink);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error at: " + url);
                insertError(url, e.ToString());
            }
        }


        public int parseHTML(string link)
        {
            Boolean addedToTable = false;
            int tableCount = 0;
            if (!duplicates.Contains(link.ToLower()))
            {
                try
                {
                    HtmlWeb web = new HtmlWeb();
                    HtmlDocument htmlDoc = web.Load(link); //check if link exists and is an html document
                    if (htmlDoc.DocumentNode != null)
                    {
                        string title = "";
                        string lowerTitle = "";
                        var titleNode = htmlDoc.DocumentNode.SelectSingleNode("//head/title");
                        if (titleNode != null)
                        {
                            title = titleNode.InnerHtml.Trim();
                            // remove title captions
                            title = title.Split(new string[] { " - CNN.com" }, StringSplitOptions.None)[0];
                            title = title.Split(new string[] { " | Bleacher Report" }, StringSplitOptions.None)[0];
                            title = title.Split(new string[] { " - CNNPolitics.com" }, StringSplitOptions.None)[0];
                            title = title.Split(new string[] { " - CNN Video" }, StringSplitOptions.None)[0];
                            title = title.Split(new string[] { " - CNNMoney" }, StringSplitOptions.None)[0];
                            title = title.Split(new string[] { " - Special Reports from CNN.com" }, StringSplitOptions.None)[0];
                            lowerTitle = title.ToLower();
                        }
                        else
                        {
                            title = "NA";
                        }
                        string date = "";
                        // @content needed?
                        var dateNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='lastmod']");
                        var dateNode2 = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='pubdate']");
                        if (dateNode != null)
                        {
                            HtmlAttribute desc = dateNode.Attributes["content"];
                            date = desc.Value;
                        }
                        else if (dateNode2 != null)
                        {
                            HtmlAttribute desc = dateNode2.Attributes["content"];
                            date = desc.Value;
                        } else
                        {
                            date = "NA";
                        }
                        //string imgUrl;
                        //string body
                        HtmlNode imgUrlNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:image']");
                        string imgUrl = imgUrlNode != null ? imgUrlNode.GetAttributeValue("content", "") : "http://i.cdn.turner.com/cnn/.e/img/4.0/logos/cnn_logo_social.jpg";

                        HtmlNode bodyNode = htmlDoc.DocumentNode.SelectSingleNode("//meta[@name='description']");
                        HtmlNode bodyNode2 = htmlDoc.DocumentNode.SelectSingleNode("//meta[@property='og:description']");
                        string body = title;
                        if (bodyNode != null)
                        {
                            HtmlAttribute desc = bodyNode.Attributes["content"];
                            body = desc.Value;
                        } else if (bodyNode2 != null)
                        {
                            HtmlAttribute desc = bodyNode2.Attributes["content"];
                            body = desc.Value;
                        }
                        //if (body.Length >= 61)
                        //{
                        //    body = body.Insert(60, "<br />");
                        //}
                        if (body.Length >= 150)
                        {
                            body = body.Substring(0, 150) + "...";
                        }
                        try
                        {
                            // insert webpages into table
                            string[] splitTitle = lowerTitle.Split(' ');
                            List<string> duplicateWords = new List<String>();
                            // new try and catch statement?
                            foreach (string word in splitTitle)
                            {
                                try
                                {
                                    Regex reg = new Regex("[^a-zA-Z]");
                                    string editedWord = reg.Replace(word, "");
                                    if (editedWord != "")
                                    {
                                        if (!duplicateWords.Contains(editedWord))
                                        {
                                            duplicateWords.Add(editedWord);
                                            try
                                            {
                                                Webpage page = new Webpage(link, title, editedWord, date, body, imgUrl);
                                                TableOperation insertOperation = TableOperation.Insert(page);
                                                urlsTable.Execute(insertOperation);
                                                tableCount++;
                                            }
                                            catch (Exception e)
                                            {
                                                Debug.WriteLine("Error at: " + link);
                                                insertError(link, e.ToString());
                                            }
                                        }
                                    }
                                }
                                catch (Exception e)
                                {
                                    Debug.WriteLine("Error at: " + link);
                                    insertError(link, e.ToString());
                                }
                            }
                            //testing
                            //Errors error = new Errors(link, "test message");
                            //TableOperation errorOperation = TableOperation.Insert(error);
                            //errorsTable.Execute(errorOperation);
                            duplicates.Add(link.ToLower());
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine("Error at: " + link);
                            insertError(link, e.ToString());
                        }
                    }
                    try
                    {
                        HtmlNode[] links = new HtmlNode[0];
                        var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");
                        if (linkNodes != null)
                        {
                            links = linkNodes.ToArray();
                        }
                        foreach (HtmlNode item in links)
                        {
                            // insert into html queue
                            string hrefValue = item.GetAttributeValue("href", string.Empty).Trim();
                            string correctUrl = "";
                            if (hrefValue.Length > 2)
                            {
                                if (hrefValue.Substring(0, 2) == "//")
                                {
                                    correctUrl = "http://" + hrefValue.Substring(2);
                                }
                                else if (hrefValue.Substring(0, 1) == "/")
                                {
                                    correctUrl = baseUrl + hrefValue;
                                }
                                else if (hrefValue.Substring(0, 4) == "http")
                                {
                                    correctUrl = hrefValue;
                                }
                                else
                                {
                                    //correctUrl = "XXX";
                                }
                                //insert into html queue
                                if (!disallows.Any(link.ToLower().StartsWith) && !duplicates.Contains(link.ToLower()))
                                {
                                    if (correctUrl.ToLower().Contains("cnn.com") || correctUrl.ToLower().Contains("bleacherreport.com/articles"))
                                    {
                                        CloudQueueMessage htmlLink = new CloudQueueMessage(correctUrl);
                                        htmlQueue.AddMessage(htmlLink);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine("Error at: " + link);
                        insertError(link, e.ToString());
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Error at: " + link);
                    insertError(link, e.ToString());
                }
            }
            return tableCount;
        }

        public void parseRobot(string url)
        {
            baseUrl = url;
            string robotsUrl = url + "/robots.txt";
            WebResponse response;
            WebRequest request = WebRequest.Create(robotsUrl);
            response = request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream());
            using (reader)
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    if (line.StartsWith("Disallow:"))
                    {
                        string item = line.Substring(10);
                        disallows.Add((baseUrl + item).ToLower());
                    }
                    else if (line.StartsWith("Sitemap:") && baseUrl == "http://www.cnn.com")
                    {
                        string item = line.Substring(9);
                        robotXmlList.Add(item);
                    }
                }
            }
            if (baseUrl == "http://bleacherreport.com")
            {
                robotXmlList.Add("http://bleacherreport.com/sitemap/nba.xml");
            }
        }

        private void insertError(string link, string message)
        {
            HtmlWeb htmlWeb = new HtmlWeb();
            Errors error = new Errors(link, message);
            TableOperation errorOperation = TableOperation.Insert(error);
            errorsTable.Execute(errorOperation);
        }
    }
}