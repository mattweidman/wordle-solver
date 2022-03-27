using System;
using System.Collections.Immutable;

namespace WordleSolver
{
    public class Program
    {
        private static readonly int WORDS_TO_SHOW = 10;

        public static void Main(string[] args)
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize();
            
            int tries = 0;
            while (true)
            {
                Console.WriteLine("Computing the top words.");
                ImmutableList<(string, double)> topWords =
                    EliminationScoreSolver.GetTopScoringWords(
                        wordsCollection.allWords, WORDS_TO_SHOW);
            
                Console.WriteLine("Rank\tWord\tScore");
                for (int i = 0; i < topWords.Count; i++)
                {
                    (string, double) pair = topWords[i];
                    Console.WriteLine($"{i + 1}\t{pair.Item1}\t{pair.Item2}");
                }
                Console.WriteLine($"There are {wordsCollection.allWords.Count} possible words.");

                Console.WriteLine("\nPlease play another word.");
                Console.WriteLine("What word did you play?");
                string guess = PromptUtils.GetGuessFromUser();

                Console.WriteLine("What was the result?\nEnter a combination of 'g', 'y', and 'r', "
                    + "where 'g' means green, 'y' means yellow, and 'r' means gray.");
                GuessResult result = PromptUtils.GetResultFromUser(guess);

                tries++;

                if (result.UserWon())
                {
                    break;
                }

                wordsCollection.EliminateWords(result);
            }
            
            Console.WriteLine($"Congratulations! You finished in {tries} tries.");
        }
    }
}
