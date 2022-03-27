using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WordleSolver
{
    /// <summary>
    /// Computes the best word based on a scoring algorithm. The score of a word
    /// is the approximate number of words we expect to be eliminated by each letter.
    /// </summary>
    public static class EliminationScoreSolver
    {
        private static readonly int ALPHABET_LENGTH = 26;
        private static readonly int WORD_LENGTH = 5;

        /// <summary>
        /// Pre-computation step.
        /// </summary>
        public static EliminationData ComputeEliminationData(IImmutableSet<string> words)
        {
            // containingCounts[i] is the number of words containing the ith letter of the
            // alphabet.
            int[] containingCounts = new int[ALPHABET_LENGTH];

            // inPositionCounts[i, j] is the number of words that have the ith letter of the
            // alphabet in position j.
            int[,] inPositionCounts = new int[ALPHABET_LENGTH, WORD_LENGTH];

            foreach (string word in words)
            {
                HashSet<int> letterNumsFound = new HashSet<int>();
                for (int pos = 0; pos < WORD_LENGTH; pos++)
                {
                    int letterNum = word[pos] - 'a';
                    inPositionCounts[letterNum, pos]++;
                    letterNumsFound.Add(letterNum);
                }

                foreach (int letterNum in letterNumsFound)
                {
                    containingCounts[letterNum]++;
                }
            }

            return new EliminationData(words.Count, containingCounts, inPositionCounts);
        }

        /// <summary>
        /// Expected number of words eliminated by a character.
        /// </summary>
        /// <param name="c">Character in word.</param>
        /// <param name="pos">Position in word.</param>
        /// <param name="eliminationData">Pre-computed data.</param>
        /// <param name="wasAlreadySeen">Whether c was already seen in the current word.</param>
        public static double GetEliminationsScoreOfChar(
            char c, int pos, EliminationData eliminationData, bool wasAlreadySeen)
        {
            int greenScore = eliminationData.wordCount -
                eliminationData.GetNumberOfWordsWithLetterInPosition(c, pos);
            int greenWeight = eliminationData.GetNumberOfWordsWithLetterInPosition(c, pos);

            if (wasAlreadySeen)
            {
                return (greenScore * greenWeight) / (double) eliminationData.wordCount;
            }

            int yellowScore = (eliminationData.wordCount -
                eliminationData.GetNumberOfWordsContaining(c)) +
                eliminationData.GetNumberOfWordsWithLetterInPosition(c, pos);
            int yellowWeight = eliminationData.GetNumberOfWordsContaining(c) - greenWeight;
            
            int grayScore = eliminationData.GetNumberOfWordsContaining(c);
            int grayWeight = eliminationData.wordCount - yellowWeight - greenWeight;

            return (greenScore * greenWeight + yellowScore * yellowWeight + grayScore * grayWeight)
                / (double)eliminationData.wordCount;
        }

        /// <summary>
        /// Expected number of words eliminated by a word.
        /// </summary>
        /// <param name="word">Word guess.</param>
        /// <param name="eliminationData">Pre-computed data.</param>
        public static double GetEliminationScoreOfWord(
            string word, EliminationData eliminationData)
        {
            HashSet<char> charsSeenSoFar = new HashSet<char>();
            double score = 0;
            for (int i = 0; i < word.Length; i++)
            {
                char c = word[i];
                score += GetEliminationsScoreOfChar(
                    c, i, eliminationData, charsSeenSoFar.Contains(c));
                charsSeenSoFar.Add(c);
            }

            return score / word.Length;
        }

        /// <summary>
        /// Compute the top-scoring words in a set of possible words.
        /// </summary>
        /// <param name="words">Set of all possible words.</param>
        /// <param name="numWords">Number of words to return.</param>
        /// <returns>List of tuples (s, x), where s is a word, and x is the word's score.</returns>
        public static ImmutableList<(string, double)> GetTopScoringWords(
            IImmutableSet<string> words, int numWords)
        {
            EliminationData eliminationData = ComputeEliminationData(words);
            return words
                .Select(word => (word, GetEliminationScoreOfWord(word, eliminationData)))
                .OrderByDescending(pair => pair.Item2)
                .Take(numWords)
                .ToImmutableList();
        }

        public class EliminationData
        {
            public readonly int wordCount;

            /// <summary>Number of words containing each letter.</summary>
            private readonly int[] containingCounts;

            /// <summary>Number of words with a letter in a particular position.</summary>
            private readonly int[,] inPositionCounts;

            public EliminationData(int wordCount, int[] containingCounts, int[,] inPositionCounts)
            {
                this.wordCount = wordCount;
                this.containingCounts = containingCounts;
                this.inPositionCounts = inPositionCounts;
            }

            public int[] GetContainingCounts()
            {
                return (int[])this.containingCounts.Clone();
            }

            public int[,] GetInPositionCounts()
            {
                return (int[,])this.inPositionCounts.Clone();
            }

            public int GetNumberOfWordsContaining(char c)
            {
                return this.containingCounts[c - 'a'];
            }

            public int GetNumberOfWordsWithLetterInPosition(char c, int pos)
            {
                return this.inPositionCounts[c - 'a', pos];
            }

            /// <summary>Human-readable summary string.</summary>
            public string GetSummary()
            {
                string containingStr = string.Join(
                    ", ",
                    this.containingCounts
                        .Select((count, pos) => (count, pos))
                        .OrderByDescending(pair => pair.count)
                        .Select(pair => $"{(char)(pair.pos + 'a')}: {pair.count}"));
                
                List<(char, int, int)> inPositionTups = new List<(char, int, int)>();
                for (int alpha = 0; alpha < this.inPositionCounts.GetLength(0); alpha++)
                {
                    for (int pos = 0; pos < this.inPositionCounts.GetLength(1); pos++)
                    {
                        inPositionTups.Add(
                            ((char)(alpha + 'a'), pos, this.inPositionCounts[alpha, pos]));
                    }
                }
                string inPositionStr = string.Join(
                    ", ",
                    inPositionTups
                        .OrderByDescending(pair => pair.Item3)
                        .Select(pair => $"({pair.Item1}, {pair.Item2}): {pair.Item3}"));

                return $"Words containing:\n{containingStr}\n"
                    + $"Words with letter in position:\n{inPositionStr}";
            }
        }
    }
}