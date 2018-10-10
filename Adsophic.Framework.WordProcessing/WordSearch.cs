using System;
using System.Collections.Generic;
using System.IO;
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
        /// Autocorrect for all dictionary words
        /// </summary>
        private AutoCorrect autoCorrect;

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
        private Lazy<Trei> treiInitializer = new Lazy<Trei>(() => InitializeTrei());

        /// <summary>
        /// Helper to initialize the single instance of the AutoCorrect
        /// </summary>
        private Lazy<AutoCorrect> autoCorrectInitializer = new Lazy<AutoCorrect>(() => InitializeAutoCorrect());


        /// <summary>
        /// Initializes WordSearch with dictionary words
        /// </summary>        
        public async Task Initialize()
        {
            if (initialized) return;

            //Start initialization in background
            await Task.Factory.StartNew(() =>
            {
                trei = treiInitializer.Value;
                autoCorrect = autoCorrectInitializer.Value;
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
            Console.WriteLine($"Initializing Trei with words");
            var trei = new Trei();
            var wordCount = 0;
            var printInterval = 300;
            using (var stream = GetWordStream())
            {
                var word = stream.ReadLine();
                while (word != null)
                {
                    if (trei.AddWord(word))
                        wordCount++;
                    if ((wordCount % printInterval) == 0)
                        Console.WriteLine($"Completed {wordCount} words");
                    word = stream.ReadLine();
                }
            }

            Console.WriteLine($"Completed Trei {wordCount} words");
            return trei;
        }

        /// <summary>
        /// Initializes an AutoCorrect object to use locally
        /// </summary>
        private static AutoCorrect InitializeAutoCorrect()
        {
            Console.WriteLine($"Initializing autocorrect with words");
            var autoCorrect = new AutoCorrect();
            var wordCount = 0;
            var printInterval = 300;
            using (var stream = GetWordStream())
            {
                var word = stream.ReadLine();
                while (word != null)
                {
                    if (autoCorrect.AddWord(word))
                        wordCount++;
                    if ((wordCount % printInterval) == 0)
                        Console.WriteLine($"Completed Autocorrect {wordCount} words");
                    word = stream.ReadLine();
                }
            }

            Console.WriteLine($"Completed autocorrect {wordCount} words");
            return autoCorrect;
        }

        private static StreamReader GetWordStream()
        {
            return new StreamReader(File.OpenRead("google-10000-english-usa.txt"));
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

        /// <summary>
        /// Finds words with the close edit distance that start with the same 
        /// letter 
        /// </summary>
        /// <param name="word">Word to match</param>
        /// <returns>IEnumerable of strings representing the words that match the search word</returns>
        public async Task<IEnumerable<string>> FindClosestMatchingWords(string word)
        {
            //Make sure initialization is performed
            await Initialize();

            //Now perform search via autocorrect
            return await autoCorrect.FindClosestWords(word);
        }
    }
}
