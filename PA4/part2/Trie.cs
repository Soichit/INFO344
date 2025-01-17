﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;





namespace WebRole1
{
    public class Trie
    {
        private TrieNode root;
        private int _hybridCapacity = 20;
        private int _searchResults = 10;
        private int _levenshteinDistance = 2;
        private Dictionary<String, int> userSearches= new Dictionary<String, int>();

        /// constructor
        public Trie()
        {
            root = new TrieNode();
        }

        public String insert(String word)
        {
            if (word == null || word.Length == 0)
            {
                return "can't do empty";
            }
            return rearrange(root, word);
        }

        private String rearrange(TrieNode current, String tempWord)
        {
            if (tempWord.Length == 0)
            {
                return "done";
            }
            char ch = tempWord[0];
            TrieNode node;

            // check that first letter is in the dictionary key
            if (current.Dict.ContainsKey(ch))
            {
                // if it does, pass that word into that node
                node = current.Dict[ch];
            }
            else
            {
                // if it doesn't, create a new node with that letter and add the rest of the letters into the hybrid
                node = new TrieNode();
            }
            if (tempWord.Length == 1)
            {
                node.SetEndOfWord(true);
            }

            // if parent node, then pass on string
            if (current.Dict.Count > 0 && tempWord.Length >= 2)
            {
                if (!current.Dict.ContainsKey(ch))
                {
                    current.Dict.Add(ch, node);
                }
                rearrange(node, tempWord.Substring(1, tempWord.Length - 1));
            }
            // if child node, 
            else if (current.Dict.Count == 0)
            {
                current.HybridList.Add(tempWord);
                // if child node reaches capacity, then rearrange
                if (current.HybridList.Count > _hybridCapacity) {
                    foreach (String item in current.HybridList)
                    {
                        ch = item[0];
                        // check that first letter is in the dictionary key
                        if (current.Dict.ContainsKey(ch))
                        {
                            // if it does, pass that word into that node
                            node = current.Dict[ch];
                        }
                        else
                        {
                            // if it doesn't, create a new node with that letter and add the rest of the letters into the hybrid
                            node = new TrieNode();
                        }
                        if (item.Length == 1)
                        {
                            node.SetEndOfWord(true);
                        }

                        if (!current.Dict.ContainsKey(ch))
                        {
                            current.Dict.Add(ch, node);
                        }
                        rearrange(node, item.Substring(1, item.Length - 1));
                    }
                    current.HybridList.Clear();
                    return "done";
                }
            }
            return "done";
        }

        public Dictionary<String, int> search(String input)
        {
            List<String> result = searchPrefix(input);
            int inputLength = 2;
            if (result.Count < _searchResults && input.Length >= inputLength)
            {
                TrieNode current = traverseTrie(root, inputLength, input);
                String inputSubstring = input.Substring(0, inputLength);
                result = searchMistakes(current, inputSubstring, input, result);
            }
            Dictionary<String, int> countResult = new Dictionary<String, int>();
            foreach(String word in result)
            {
                String processedWord = word.Replace('_', ' ');
                int count = userSearches.ContainsKey(processedWord) ? userSearches[processedWord] : 0;
                if (!countResult.ContainsKey(processedWord))
                {
                    countResult.Add(processedWord, count);
                }
            }
            var sortedDict = from entry in countResult orderby entry.Value descending select entry;
            var result2 = sortedDict.ToDictionary(pair => pair.Key, pair => pair.Value);
            return result2;
        }

        private List<String> searchPrefix(String input)
        {
            List<String> result = new List<String>();
            TrieNode current = root;
            String tempWord = "";
            char ch = '_';
            if (input == null || input.Length == 0)
            {
                return result;
            }
            for (int i = 0; i < input.Length; i++)
            {
                foreach (String word in current.HybridList)
                {
                    String subtractedWord = input;
                    if (tempWord != "")
                    {
                        subtractedWord = input.Replace(tempWord, "");
                    }
                    if (word.StartsWith(subtractedWord))
                    {
                        if (!result.Exists(x => x == tempWord + word))
                        {
                            result.Add(tempWord + word);
                        }
                        if (result.Count == _searchResults)
                        {
                            return result;
                        }
                    }
                }
                ch = input[i];
                if (!current.Dict.ContainsKey(ch))
                {
                    return result;
                }
                else
                {
                    tempWord += ch;
                    current = current.Dict[ch];
                }
            }
            return searchSuggestions(current, tempWord, input, result);
        }

        private List<String> searchSuggestions(TrieNode current, String tempWord, String input, List<String> result)
        {
            if (result.Count >= _searchResults)
            {
                return result;
            }
            else
            {
                TrieNode node = current;
                if (current.EndOfWord)
                {
                    if (!result.Exists(x => x == tempWord))
                    {
                        result.Add(tempWord);
                    }
                    if (result.Count >= _searchResults)
                    {
                        return result;
                    }
                }

                foreach (KeyValuePair<char, TrieNode> item in node.Dict)
                {
                    char ch = item.Key;
                    TrieNode nextNode = item.Value;
                    result = searchSuggestions(nextNode, tempWord + ch, input, result);
                    if (result.Count >= _searchResults)
                    {
                        return result;
                    }
                }

                foreach (String word in current.HybridList)
                {
                    if (!result.Exists(x => x == tempWord + word))
                    {
                        result.Add(tempWord + word);
                    }
                    if (result.Count >= _searchResults)
                    {
                        return result;
                    }
                }
                return result;
            }
        }

        private TrieNode traverseTrie(TrieNode current, int index, String input)
        {
            for (int i = 0; i < index; i++)
            {
                char ch = input[i];
                if (current.Dict.ContainsKey(ch))
                {
                    current = current.Dict[ch];
                } else
                {
                    return null;
                }
            }
            return current;
        }

        private List<String> searchMistakes(TrieNode current, String tempWord, String input, List<String> result)
        {
            if (result.Count == _searchResults)
            {
                return result;
            }
            else
            {
                TrieNode node = current;
                if (current != null && current.EndOfWord)
                {
                    if (result.Count < _searchResults)
                    {
                        if (levenshtein(tempWord, input) <= _levenshteinDistance)
                        {
                            if (!result.Exists(x => x == tempWord))
                            {
                                result.Add("Did you mean: '" + tempWord + "'");
                            }
                        }
                    }
                }

                foreach (KeyValuePair<char, TrieNode> item in node.Dict)
                {
                    char ch = item.Key;
                    TrieNode nextNode = item.Value;
                    result = searchMistakes(nextNode, tempWord + ch, input, result);
                    if (result.Count == _searchResults)
                    {
                        return result;
                    }
                }

                foreach (String word in current.HybridList)
                {
                    String addedWord = tempWord + word;
                    if (levenshtein(addedWord, input) <= _levenshteinDistance)
                    {
                        if (!result.Exists(x => x == addedWord))
                        {
                            result.Add("'" + addedWord + "'");
                        }
                        if (result.Count == _searchResults)
                        {
                            return result;
                        }
                    }
                }
                return result;
            }
        }

        private int levenshtein(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            // Step 1
            if (n == 0)
            {
                return m;
            }

            if (m == 0)
            {
                return n;
            }

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
        
        public String addUserSearch(String word)
        {
            TrieNode current = traverseTrie(root, word.Length, word);
            if (current == null)
            {
                return null;
            }

            if (!userSearches.ContainsKey(word))
            {
                userSearches.Add(word, 1);
            }
            else
            {
                userSearches[word] = userSearches[word] + 1;
            }
            return "adding count to search done!";
        }
    }
}
