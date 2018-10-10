using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Adsophic.Framework.WordProcessing
{
    /// <summary>
    /// Maintains Trei of all words 
    /// </summary>
    internal class Trei
    {
        private TreiNode root;

        /// <summary>
        /// Create a Trei
        /// </summary>
        internal Trei()
        {
            root = new TreiNode((char)0);
        }

        /// <summary>
        /// Adds word to Trei
        /// </summary>
        /// <param name="word">Word to add</param>
        /// <returns>true if word was added, false otherwise, returns false if word already exists</returns>
        internal bool AddWord(string word)
        {
            //Cannot add empty or null string
            if ((word?.Length ?? 0) == 0) throw new ArgumentException("Word canot be empty or null");

            var curNode = root;
            var isNewWord = false;
            //Used to keep track of nodes traversed. We want to keep track of how 
            //many words go through a particular node. 
            //We will increment number of words in the end 
            //if new word was created
            var traversedNodes = new List<TreiNode>();
            traversedNodes.Add(root);

            //loop through each character in the string
            foreach(var c in word.ToLower())
            {
                //Check if child node for character exists
                if (!curNode.Children.TryGetValue(c, out TreiNode node))
                {
                    //Add new character if it does not currently exist
                    node = new TreiNode(c);
                    curNode.Children[c] = node;
                    //if a new node is created this is a new word
                    isNewWord = true;
                }

                //traverse down to new or existing node that represents the next character. 
                curNode = node;

                //Add node to list of traversed nodes
                traversedNodes.Add(curNode);
            }

            //Update word count for each traversed node if this is a new word. 
            //If an existing word is added isNewWord will remain false
            if (isNewWord) traversedNodes.ForEach(t => t.WordCount++);

            //Mark last node as IsWord. This is required 
            //Example: When we add word "cat" we don't want the FindWord to return
            //true if we search for "ca". By setting IsWord on last node only
            //we can determine that if we end up on node representing character "a" 
            //from "ca" it is not a word
            traversedNodes[traversedNodes.Count - 1].IsWord = true;
            return isNewWord;
        }

        /// <summary>
        /// Find a word in trei
        /// </summary>
        /// <param name="word">Word to search</param>
        /// <returns>true if word is found, false otherwise</returns>
        internal async Task<bool> FindWord(string word)
        {
            //empty and null words are not in trei
            if ((word?.Length ?? 0) == 0) return false;            

            var result = await Task.Factory.StartNew(() =>
            {
                var curNode = root;
                //Iterate character by character
                foreach (var c in word.ToLower())
                {
                    //Look for existence of child character. 
                    //return false if character cannot be found
                    if (!curNode.Children.TryGetValue(c, out TreiNode node))
                        return null;
                    curNode = node;
                }

                return curNode;
            });

            //We have traversed all characters and we have continued to find new nodes. 
            //But, but but, we cannot simply return true in that case. 
            //The last node must also have IsWord set to true to signify that it 
            //does indeed represent the last character in a word
            return result?.IsWord ?? false;
        }

        /// <summary>
        /// Takes a string representing an array of characters and 
        /// returns a list of words that can be constructed using 
        /// those words
        /// </summary>
        /// <param name="characters">string representing array of characters</param>
        /// <param name="wordMinimumSize">minimum size word you want to include in results, defaults to 1</param>
        /// <returns>enumeration of strings containing all words that can be constructed using the array of input characters</returns>
        internal async Task<IEnumerable<string>> FindWordsFromCharacters(string characters, int wordMinimumSize = 1)
        {
            //return empty list of string in
            if ((characters?.Length ?? 0) == 0) return new string[0];

            //set to store all words found. We use set instead 
            //of a list to restrict output to distinct words
            var results = new HashSet<string>();

            //Internal method to aid recursive search for words
            void wordSearchHelper(string charactersSoFar, TreiNode curNode, string remaining)
            {
                //if curnode has IsWord set to true we have found a word. 
                //Add it to the results. We still have to continue searching
                //for searching. This will ensure tha means that "a" and "an" can both be in
                //included in the result. Only include the word in results if Length satisfies the wordMinimumSize 
                //condition
                if (curNode.IsWord && charactersSoFar.Length >= wordMinimumSize) results.Add(charactersSoFar);
                //If we reached the end of the list of characters return
                if (remaining == string.Empty) return;

                //This is the standard string permutations algorithm with a slight twist. 
                //The main difference is the we stop if we don't find a child node for 
                //character tested.
                foreach (var characterAndIndex in remaining.Select((character, index) => new { character, index }))
                {
                    //Search for child representing current character
                    if (curNode.Children.TryGetValue(characterAndIndex.character, out TreiNode node))
                    {
                        //If character is found traverse to that character
                        //recurse for rest of the characters
                        wordSearchHelper(charactersSoFar + characterAndIndex.character, node,
                            remaining.Substring(0, characterAndIndex.index) + remaining.Substring(characterAndIndex.index + 1));
                    }
                }
            }

            //Start search for words.
            await Task.Factory.StartNew(() => wordSearchHelper(string.Empty, root, characters));

            //return results
            return results;
        }

        /// <summary>
        /// TreiNode represents an individual character in a Trei
        /// </summary>
        private class TreiNode
        {
            //Use Lazy so dictionaries are not initialized for leaf level nodes. 
            private Lazy<Dictionary<char, TreiNode>> children =
                new Lazy<Dictionary<char, TreiNode>>(() => new Dictionary<char, TreiNode>());

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="tchar">character representing this Trei</param>
            public TreiNode(char tchar)
            {
                Char = tchar;
            }

            /// <summary>
            /// Char represented by Trei node
            /// </summary>
            public char Char { get; }

            /// <summary>
            /// Should be set to true if this node represents the last 
            /// character in a word
            /// </summary>
            public bool IsWord { get; set; }

            /// <summary>
            /// Children characters
            /// </summary>
            public Dictionary<char, TreiNode> Children { get { return children.Value; } }

            /// <summary>
            /// Represents the number of words that traverse through this node.
            /// For a Trei with two words "a" and "an" node representing 'a' 
            /// will have its WordCount set to 2, for node representing 'n'
            /// this will be set to 1
            /// </summary>
            public int WordCount { get; set; }
        }
    }
}
