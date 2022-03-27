using System.Collections.Generic;
using System.Collections.Immutable;

namespace WordleSolver
{
    public class WordsCollection
    {
        private static readonly string WORDS_FILE_PATH = "words.txt";

        public IImmutableSet<string> allWords;

        private WordsCollection(IEnumerable<string> allWords)
        {
            this.allWords = allWords.ToImmutableHashSet();
        }

        public static WordsCollection Initialize()
        {
            string[] lines = System.IO.File.ReadAllLines(WORDS_FILE_PATH);
            return new WordsCollection(lines);
        }
    }
}