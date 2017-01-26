using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PA2
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Dict { get; private set; }
        public Boolean EndOfWord { get; private set; }


        public TrieNode()
        {
            this.Dict = new Dictionary<char, TrieNode>();
            this.EndOfWord = false;
        }

        public TrieNode(Dictionary<char, TrieNode> dict, Boolean endOfWord)
        {
            this.Dict = dict;
            this.EndOfWord = endOfWord;
        }

        public void SetDict(Dictionary<char, TrieNode> newDict)
        {
            this.Dict = newDict;
        }

        public void addToDict(char ch, TrieNode node)
        {
            this.Dict.Add(ch, node);
        }


        public void SetEndOfWord(Boolean newEndOfWord)
        {
            this.EndOfWord = newEndOfWord;
        }
    }
}