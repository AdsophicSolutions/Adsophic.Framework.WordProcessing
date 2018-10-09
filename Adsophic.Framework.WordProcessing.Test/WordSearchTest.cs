using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Adsophic.Framework.WordProcessing.Test
{
    [TestClass]
    public class WordSearchTest
    {  
        [TestMethod]
        public void WordSearchInitTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            Assert.IsTrue(wordSearch.Initialized);
        }

        [TestMethod]
        public void WordSearchFindWordTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            Assert.IsTrue(wordSearch.FindWord("cat").GetAwaiter().GetResult());
        }

        [TestMethod]
        public void WordSearchWordNotFoundTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            Assert.IsFalse(wordSearch.FindWord("lion").GetAwaiter().GetResult());
        }

        [TestMethod]
        public void WordSearchWordFromCharactersResultCountTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();
            
            Assert.AreEqual(expected: 2, actual: wordSearch.FindWordsFromCharacters("na").GetAwaiter().GetResult().Count());
        }

        [TestMethod]
        public void WordSearchWordFromCharactersResultsTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            var results = wordSearch.FindWordsFromCharacters("na").GetAwaiter().GetResult().ToHashSet();
            Assert.AreEqual(expected: 2, actual: results.Count);
            Assert.IsTrue(results.Contains("a"));
            Assert.IsTrue(results.Contains("an"));
        }

        [TestMethod]
        public void WordSearchWordFromCharactersResultsMinSizeTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            var results = wordSearch.FindWordsFromCharacters("na", 2).GetAwaiter().GetResult().ToHashSet();
            Assert.AreEqual(expected: 1, actual: results.Count);            
            Assert.IsTrue(results.Contains("an"));
        }

        [TestMethod]
        public void WordSearchWordFromCharactersResultsMinSizeNoResultsTest()
        {
            var wordSearch = WordSearch.Instance;
            wordSearch.Initialize().GetAwaiter().GetResult();

            var results = wordSearch.FindWordsFromCharacters("na", 3).GetAwaiter().GetResult().ToHashSet();
            Assert.AreEqual(expected: 0, actual: results.Count);
        }
    }
}
