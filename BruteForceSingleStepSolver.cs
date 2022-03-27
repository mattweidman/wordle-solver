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
            foreach (string guess in possibleWords)
            {
                int numberRemaining = 0;
                foreach (string solution in possibleWords)
                {
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