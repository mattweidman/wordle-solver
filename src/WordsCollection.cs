using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WordleSolver
{
    public class WordsCollection
    {
        private static readonly string WORDS_FILE_PATH = "words.txt";

        private HashSet<string> currentWordsMutable;

        public readonly IImmutableSet<string> allWords;

        public IImmutableSet<string> currentWords => currentWordsMutable.ToImmutableHashSet();

        private WordsCollection(IEnumerable<string> allWords)
        {
            this.allWords = allWords.ToImmutableHashSet();
            this.currentWordsMutable = allWords.ToHashSet();
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
            this.currentWordsMutable =
                this.currentWordsMutable.Where(word => result.Accepts(word)).ToHashSet();
        }

        /// <summary>
        /// Whether the word is anywhere in the Wordle dictionary.
        /// </summary>
        public bool IsInDictionary(string word)
        {
            return this.allWords.Contains(word);
        }
    }
}