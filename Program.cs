using System;

namespace WordleSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            WordsCollection wordsCollection = WordsCollection.Initialize();

            Console.WriteLine("Computing the best starting Wordle word.");
            string bestWord = BruteForceSingleStepSolver.BestGuess(wordsCollection.allWords);
            Console.WriteLine(bestWord);
        }
    }
}
