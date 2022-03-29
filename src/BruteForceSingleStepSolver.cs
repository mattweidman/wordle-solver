using System;
using System.Collections.Immutable;

namespace WordleSolver
{
    /// <summary>
    /// This is too slow. Do not use.
    /// </summary>
    public static class BruteForceSingleStepSolver
    {
        public static string BestGuess(IImmutableSet<string> possibleWords)
        {
            int lowestScore = possibleWords.Count;
            string bestWord = "";
            int guessesSoFar = 0;
            foreach (string guess in possibleWords)
            {
                guessesSoFar++;
                Console.WriteLine($"Guess {guessesSoFar}: {guess}");
                int numberRemaining = 0;
                int solutionsSoFar = 0;
                foreach (string solution in possibleWords)
                {
                    solutionsSoFar++;
                    Console.WriteLine($"\tSolution {solutionsSoFar}: {solution}");
                    GuessResult result = GuessResult.FromGuessAndSolution(guess, solution);
                    foreach (string word in possibleWords)
                    {
                        if (result.Accepts(word))
                        {
                            numberRemaining++;
                        }
                    }
                }

                if (numberRemaining < lowestScore)
                {
                    lowestScore = numberRemaining;
                    bestWord = guess;
                }
            }

            return bestWord;
        }
    }
}