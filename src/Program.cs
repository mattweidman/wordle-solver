using System;
using System.Collections.Generic;
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
                ImmutableList<(string, double)> topWordsOfCurrent =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, WORDS_TO_SHOW, false);
                ImmutableList<(string, double)> topWordsOfAll =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, WORDS_TO_SHOW, true);
                Console.WriteLine("All top words:");
                displayTopWords(topWordsOfAll);
                Console.WriteLine("Top words that have not been eliminated:");
                displayTopWords(topWordsOfCurrent);
                Console.WriteLine(
                    $"There are {wordsCollection.currentWords.Count} possible words.");

                Console.WriteLine("\nPlease play another word.");
                Console.WriteLine("What word did you play?");
                string guess = PromptUtils.GetGuessFromUser();

                Console.WriteLine("What was the result?\n"
                    + "Enter a combination of 'g', 'y', and 'r', "
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

        private static void displayTopWords(IList<(string, double)> topWords) {
            Console.WriteLine("Rank\tWord\tScore");
            for (int i = 0; i < topWords.Count; i++)
            {
                (string, double) pair = topWords[i];
                Console.WriteLine($"{i + 1}\t{pair.Item1}\t{pair.Item2}");
            }
        }
    }
}
