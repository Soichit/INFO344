using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace WebRole1
{
    public class Stats: TableEntity
        
    {
        public string state { get; set; }
        public string lastTen { get; set; }
        public string crawled { get; set; }
        public int queueSize { get; set; }
        public int tableSize { get; set; }
        public int URLcounter { get; set; }
        public int CPUCounter { get; set; }
        public int MemCounter { get; set; }
        public int wikiTitles { get; set; }
        public string wikiLastTitle { get; set; }


        public Stats() {
            this.PartitionKey = "WikiPartionKey";
            this.RowKey = "RowKey";
            this.wikiLastTitle = "";
            this.wikiTitles = 0;
        }


        public Stats(float CPUCounter, float MemCounter) {
            this.PartitionKey = "PartionKey";
            this.RowKey = "RowKey";
            //this.RowKey = "path";
            this.state = "Idle";
            this.lastTen = "";
            this.crawled = "";
            this.queueSize = 0;
            this.tableSize = 0;
            this.URLcounter = 0;
            this.CPUCounter = (int) CPUCounter;
            this.MemCounter = (int) MemCounter;
        }

        public void updateAllStats(float CPUCounter, float MemCounter, int queueSize, int tableCount, List<string> list)
        {     
            this.CPUCounter = (int) CPUCounter;
            this.MemCounter = (int) MemCounter;
            this.queueSize = queueSize;
            this.URLcounter++;
            //increment table size by number of words
            this.tableSize += tableCount;
            updateLastTen(list);
        }

        public void updateState(string state)
        {
            this.state = state;
        }

        public void updateLastTen(List<string> list)
        {
            this.lastTen = "";
            foreach (string s in list)
            {
                this.lastTen += s + "; ";
            }
        }

        public void updateCrawled(string url)
        {
            this.crawled += url + ",";
        }

        public void updatePerformance(float CPUCounter, float MemCounter)
        {
            this.CPUCounter = (int) CPUCounter;
            this.MemCounter = (int) MemCounter;
        }

        public void updateQueue(int queueSize)
        {
            this.queueSize = queueSize;
        }

        public void updateWiki(string wikiLastTitle)
        {
            this.wikiLastTitle = wikiLastTitle;
            this.wikiTitles++;
        }
    }
}
