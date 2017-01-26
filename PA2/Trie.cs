using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PA2
{
    public class Trie
    {
        private static TrieNode root;

        /// constructor
        public Trie()
        {
            root = new TrieNode();
        }

        public void insert(String word)
        {
            TrieNode current = root;
            for (int i = 0; i < word.Length; i++)
            {
                char ch = word[i];
                TrieNode node;
                // if the char key is not already in the dictionary
                if (!current.Dict.ContainsKey(ch))
                {
                    node = new TrieNode();
                    current.Dict.Add(ch, node);
                } else
                {
                    node = current.Dict[ch];
                }
                current = node;
            }
            current.SetEndOfWord(true);
        }


        public List<String> search(String input)
        {
            TrieNode current = searchPrefix(root, input, 0);
            List<String> list = new List<String>();

            /// if passed in prefix doesn't exist, then return empty list
            if (current == null)
            {
                return list;
            } else
            {
                return searchSuggestions(current, input, list);
            }
        }

        private TrieNode searchPrefix(TrieNode current, String prefix, int index)
        {
            if (index == prefix.Length)
            {
                return current;
            }
            char ch = prefix[index];
            TrieNode node = current.Dict[ch];
            /// if passed in prefix doesn't exist
            if (node == null)
            {
                return null;
            } else
            {
                return searchPrefix(node, prefix, index + 1);
            }
        }

        private List<String> searchSuggestions(TrieNode current, String tempWord, List<String> list)
        {
            if (list.Count >= 10)
            {
                return list;
            } else
            { 
                TrieNode node = current;
                if (current.EndOfWord)
                {
                    list.Add(tempWord);
                }
                //if
                foreach(KeyValuePair<char, TrieNode> item in node.Dict)
                {
                    char ch = item.Key;
                    TrieNode nextNode = item.Value;                        
                    list = searchSuggestions(nextNode, tempWord + ch, list);
                }
                return list;
            }
        }
    }
}