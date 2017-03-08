using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebRole1
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Dict { get; private set; }
        public Boolean EndOfWord { get; private set; }
        public List<String> HybridList { get; private set; }

        public TrieNode()
        {
            this.Dict = new Dictionary<char, TrieNode>();
            this.EndOfWord = false;
            this.HybridList = new List<String>();
        }

        public TrieNode(Dictionary<char, TrieNode> dict, Boolean endOfWord, List<String> hybridList)
        {
            this.Dict = dict;
            this.EndOfWord = endOfWord;
            this.HybridList = hybridList;
        }

        public void addToDict(char ch, TrieNode node)
        {
            this.Dict.Add(ch, node);
        }

        public void addToList(String s)
        {
            this.HybridList.Add(s);
        }

        public void SetEndOfWord(Boolean newEndOfWord)
        {
            this.EndOfWord = newEndOfWord;
        }
    }
}