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

            return new EliminationData(containingCounts, inPositionCounts);
        }

        public class EliminationData
        {
            /// <summary>Number of words containing each letter.</summary>
            private readonly int[] containingCounts;

            /// <summary>Number of words with a letter in a particular position.</summary>
            private readonly int[,] inPositionCounts;

            public EliminationData(int[] containingCounts, int[,] inPositionCounts)
            {
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