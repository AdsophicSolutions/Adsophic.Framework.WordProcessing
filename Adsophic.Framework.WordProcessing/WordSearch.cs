using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Adsophic.Framework.WordProcessing
{
    /// <summary>
    /// Performs Word Search
    /// </summary>
    public class WordSearch
    {
        /// <summary>
        /// Singleton Instance
        /// </summary>
        private static Lazy<WordSearch> wordSearch = new Lazy<WordSearch>(() => new WordSearch());

        /// <summary>
        /// maintains initialization state
        /// </summary>
        private bool initialized;
        
        /// <summary>
        /// Trei representing all dictionary words
        /// </summary>
        private Trei trei;

        /// <summary>
        /// Access to Singleton instance
        /// </summary>
        public static WordSearch Instance
        {
            get { return wordSearch.Value; }
        }

        /// <summary>
        /// Helper to create a single instance of Trei
        /// </summary>
        private Lazy<Trei> initializer = new Lazy<Trei>(() => InitializeTrei());

        /// <summary>
        /// Initializes WordSearch with dictionary words
        /// </summary>        
        public async Task Initialize()
        {
            if (initialized) return;

            //Start initialization in background
            await Task.Factory.StartNew(() =>
            {
                trei = initializer.Value;
                initialized = true;
            }
            );
        }

        /// <summary>
        /// Initialized state of word search
        /// </summary>
        public bool Initialized => initialized;

        /// <summary>
        /// Method to help initilize
        /// </summary>
        private static Trei InitializeTrei()
        {
            var trei = new Trei();
            trei.AddWord("dog");
            trei.AddWord("cat");
            trei.AddWord("camel");
            trei.AddWord("goat");
            trei.AddWord("sheep");
            trei.AddWord("cow");
            trei.AddWord("ram");
            trei.AddWord("a");
            trei.AddWord("an");

            return trei;
        }

        /// <summary>
        /// Performs Word Search
        /// </summary>
        /// <param name="word">Word to search</param>
        /// <returns>true if word is found, false otherwise</returns>
        public async Task<bool> FindWord(string word)
        {
            //Make sure word search is initialized
            await Initialize();

            //Now perform search
            return await trei.FindWord(word);
        }

        /// <summary>
        /// Performs word search using characters passed in.
        /// </summary>
        /// <param name="characters">characters to search as string</param>
        /// <param name="wordMinimumSize">restrict search to word of given size or larger</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> FindWordsFromCharacters(string characters, int wordMinimumSize = 1)
        {
            //Make sure word search is initialized
            await Initialize();

            //Now perform search 
            return await trei.FindWordsFromCharacters(characters, wordMinimumSize);
        }
    }
}
