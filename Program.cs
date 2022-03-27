using System;

namespace WordleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize();

            Console.WriteLine("Pre-computing.");
            EliminationScoreSolver.EliminationData eliminationData =
                EliminationScoreSolver.ComputeEliminationData(wordsCollection.allWords);
            Console.WriteLine(eliminationData.GetSummary());
        }
    }
}
