using System;
using System.Collections.Immutable;

namespace WordleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize();

            Console.WriteLine("Computing the top words.");
            ImmutableList<(string, double)> topWords =
                EliminationScoreSolver.GetTopScoringWords(wordsCollection.allWords, 10);
            
            Console.WriteLine("Rank\tWord\tScore");
            for (int i = 0; i < topWords.Count; i++)
            {
                (string, double) pair = topWords[i];
                Console.WriteLine($"{i + 1}\t{pair.Item1}\t{pair.Item2}");
            }
        }
    }
}
