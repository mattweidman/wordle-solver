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
            if (args.Length == 0)
            {
                RunInteractive();
                return;
            }
            else if (args.Length == 3)
            {
                if (args[0] == "simulate")
                {
                    if (args[1] == "eliminate")
                    {
                        SimulateEliminate(args[2]);
                        return;
                    }
                }
            }

            Console.WriteLine("Invalid arguments. Valid commands:");
            Console.WriteLine("dotnet run");
            Console.WriteLine("dotnet run simulate eliminate <word>");
        }

        /// <summary>
        /// Display the Wordle results if the top-scoring word is taken each turn, eliminating
        /// invalid words each run so that invalid words are never played.
        /// </summary>
        private static void SimulateEliminate(string solution)
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize();

            int tries = 0;
            while (wordsCollection.currentWords.Count > 0)
            {
                string guess =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, 1, false)[0].Item1;
                GuessResult result = GuessResult.FromGuessAndSolution(guess, solution);
                Console.WriteLine(result);

                tries++;
                if (result.UserWon())
                {
                    break;
                }

                wordsCollection.EliminateWords(result);
            }

            Console.WriteLine($"Completed in {tries} tries.");
        }

        private static void RunInteractive() {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize();
            
            int tries = 0;
            while (true)
            {
                Console.WriteLine("Computing the top words.");
                ImmutableList<(string, double)> topWordsOfAll =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, WORDS_TO_SHOW, true);
                ImmutableList<(string, double)> topWordsOfCurrent =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, WORDS_TO_SHOW, false);
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
