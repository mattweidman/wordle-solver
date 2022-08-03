using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WordleSolver
{
    public class WordsCollection
    {
        private HashSet<string> currentWordsMutable;

        public readonly IImmutableSet<string> allWords;

        public IImmutableSet<string> currentWords => currentWordsMutable.ToImmutableHashSet();

        private WordsCollection(IEnumerable<string> allWords)
        {
            this.allWords = allWords.ToImmutableHashSet();
            this.currentWordsMutable = allWords.ToHashSet();
        }

        public static WordsCollection Initialize(string filePath)
        {
            string[] lines = System.IO.File.ReadAllLines(filePath);
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

        public ImmutableHashSet<string> GetRandomSubset(int size)
        {
            IImmutableList<string> allWordsList = this.allWords.ToImmutableList();
            HashSet<string> subset = new HashSet<string>();

            while (subset.Count < size)
            {
                Random rand = new Random();
                int index = rand.Next(allWordsList.Count);
                subset.Add(allWordsList[index]);
            }

            return subset.ToImmutableHashSet();
        }
    }
}