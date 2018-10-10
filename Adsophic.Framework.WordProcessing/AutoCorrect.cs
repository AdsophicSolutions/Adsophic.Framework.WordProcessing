using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adsophic.Framework.WordProcessing
{
    internal class AutoCorrect
    {
        private Dictionary<int, Dictionary<char, HashSet<string>>> wordsDictionary =
            new Dictionary<int, Dictionary<char, HashSet<string>>>();
        //private const int largestDistanceToSearch = 3;

        internal bool AddWord(string word)
        {
            if ((word?.Length ?? 0) == 0) return false;
            word = word.ToLower();
            
            //Make sure the dictionary for lengths exists
            if(!wordsDictionary.TryGetValue(word.Length, 
                out Dictionary<char, HashSet<string>> wordLengthsDictionary))
            {
                wordLengthsDictionary = new Dictionary<char, HashSet<string>>();
                wordsDictionary[word.Length] = wordLengthsDictionary;
            }

            //Look for list of words beginning with the character of 
            //the word we want to add 
            if(!wordLengthsDictionary.TryGetValue(word[0], out HashSet<string> words))
            {
                words = new HashSet<string>();
                wordLengthsDictionary[word[0]] = words;
            }

            //return false if word already exists in HashSet
            if (words.Contains(word)) return false; 

            //Add new word to HashSet
            words.Add(word);
            return true; 
        }

        /// <summary>
        /// returns closest matching words from the dictionary
        /// </summary>
        /// <param name="word">Word to search</param>
        /// <returns>Enumerable list of matching words</returns>
        internal async Task<IEnumerable<string>> FindClosestWords(string word)
        {
            if ((word?.Length ?? 0) == 0) return new string[0];
            word = word.ToLower();

            //Run search asynchronously
            return await Task.Factory.StartNew(() =>
            {
                //Find length of word to search
                var wordLength = word.Length;
                //We are going to look for words that belong to the same starting letter 
                //as the word we are searching for and length equal to or less or greater 
                //by one
                var wordsToTest =
                    FindWordBySizeAndCharacter(wordLength, word[0])
                    .Union(FindWordBySizeAndCharacter(wordLength - 1, word[0]))
                    .Union(FindWordBySizeAndCharacter(wordLength + 1, word[0]))
                    .ToArray();

                //We need a concurrent collection to make sure we can add matching
                //words in parallel
                var concurrentResult = new ConcurrentBag<Tuple<string, int>>();

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 1 };
                //Calculate distance in parallel. 
                Parallel.ForEach(wordsToTest, parallelOptions, (wordToTest) =>
                 {
                     //Find distance for the word
                     var distance = GetEditDistance(word, wordToTest);
                     //Restricting the number of edits. Vary it 
                     //by word size, but don't make it greater than 
                     //4. For large words distance cannot be 
                     //more than 4
                     var maxDistanceForWord = Math.Min(
                         Math.Max(word.Length / 2 - 1, 1), 
                         4);

                     //We cut off the search if edit distance is equal to largest distance to search.
                     //We want to only include words where edit distance is less than the largest 
                     //distance we are searching for
                     if (distance <= maxDistanceForWord)
                         concurrentResult.Add(new Tuple<string, int>(wordToTest, distance));
                 });

                //sort by distance. We want the words with minimum edit distance to 
                //appear first. 
                return concurrentResult.OrderBy(t => t.Item2).Select(t => t.Item1);
            }
            );
        }

        /// <summary>
        /// Finds words that are of the given size for a given character
        /// </summary>
        /// <param name="wordLength">Length of the word to search</param>
        /// <param name="firstChar">firstChar to search</param>
        /// <returns>IEnumerable string of word for that letter that belong to </returns>
        private IEnumerable<string> FindWordBySizeAndCharacter(int wordLength, char firstChar)
        {
            if(wordsDictionary.TryGetValue(wordLength, out Dictionary<char, HashSet<string>> wordsByLength) &&
                wordsByLength.TryGetValue(firstChar, out HashSet<string> wordsForChar))
            {
                return wordsForChar;
            }

            return new string[0];
        }

        /// <summary>
        /// Levisthan edit distance algorithm
        /// </summary>
        /// <param name="word1">Compare word one</param>
        /// <param name="word2">Compare word two</param>        
        /// <returns>The minimum number of edits it would take to convert one word to the other</returns>
        private static int GetEditDistance(string word1, string word2)
        {   
            if ((word1?.Length ?? 0) == 0 && (word2?.Length ?? 0) == 0) return 0;
            if ((word1?.Length ?? 0) == 0) return word2.Length;
            if ((word2?.Length ?? 0) == 0) return word1.Length;
            if (word1 == word2) return 0;

            var dpMatrix = new int[word1.Length + 1, word2.Length + 1];
            for (var i = 0; i < dpMatrix.GetLength(0); i++) dpMatrix[i, 0] = i;
            for (var i = 0; i < dpMatrix.GetLength(1); i++) dpMatrix[0, i] = i;

            for(var i = 1; i < dpMatrix.GetLength(0); i++)
                for(var j = 1; j < dpMatrix.GetLength(1); j++)
                {
                    if (word1[i - 1] == word2[j - 1])
                        dpMatrix[i, j] = dpMatrix[i - 1, j - 1];
                    else
                        dpMatrix[i, j] = new[]
                        {
                            dpMatrix[i - 1, j],
                            dpMatrix[i - 1, j - 1],
                            dpMatrix[i, j - 1]
                        }.Min() + 1;
                }

            return dpMatrix[dpMatrix.GetLength(0) - 1, dpMatrix.GetLength(1) - 1];
        }
    }
}
