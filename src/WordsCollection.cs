using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WordleSolver
{
    public class WordsCollection
    {
        private static readonly string WORDS_FILE_PATH = "words.txt";

        private HashSet<string> wordsSet;

        public IImmutableSet<string> allWords => wordsSet.ToImmutableHashSet();

        private WordsCollection(IEnumerable<string> allWords)
        {
            this.wordsSet = allWords.ToHashSet();
        }

        public static WordsCollection Initialize()
        {
            string[] lines = System.IO.File.ReadAllLines(WORDS_FILE_PATH);
            return new WordsCollection(lines);
        }

        /// <summary>
        /// Eliminates words based on the result from a guess.
        /// </summary>
        public void EliminateWords(GuessResult result)
        {
            this.wordsSet = this.wordsSet.Where(word => result.Accepts(word)).ToHashSet();
        }
    }
}