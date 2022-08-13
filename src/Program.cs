using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace WordleSolver
{
    public class Program
    {
        private static readonly int WORDS_TO_SHOW = 10;

        private static readonly string WORDLE_FILE = "words.txt";

        private static readonly string BIRDLE_FILE = "birds.txt";

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                RunInteractive(WORDLE_FILE);
                return;
            }
            else if (args.Length == 1)
            {
                if (args[0] == "bird")
                {
                    RunInteractive(BIRDLE_FILE);
                    return;
                }
            }
            else if (args.Length == 2)
            {
                if (args[0] == "simulate" && args[1] == "compare")
                {
                    SimulateCompare();
                    return;
                }
                else if (args[0] == "best")
                {
                    BestOrWorst(WORDLE_FILE, Int32.Parse(args[1]), false);
                    return;
                }
                else if (args[0] == "worst")
                {
                    BestOrWorst(WORDLE_FILE, Int32.Parse(args[1]), true);
                    return;
                }
            }
            else if (args.Length == 3)
            {
                if (args[0] == "simulate")
                {
                    if (args[1] == "validonly")
                    {
                        SimulateValidOnly(args[2]);
                        return;
                    }
                    else if (args[1] == "maxeliminations")
                    {
                        SimulateMaximizeEliminations(args[2]);
                        return;
                    }
                }
                else if (args[0] == "bird")
                {
                    if (args[1] == "best")
                    {
                        BestOrWorst(BIRDLE_FILE, Int32.Parse(args[2]), false);
                        return;
                    }
                    else if (args[1] == "worst")
                    {
                        BestOrWorst(BIRDLE_FILE, Int32.Parse(args[2]), true);
                        return;
                    }
                }
            }

            Console.WriteLine("Invalid arguments. Valid commands:");
            Console.WriteLine("dotnet run");
            Console.WriteLine("dotnet run best <number>");
            Console.WriteLine("dotnet run worst <number>");
            Console.WriteLine("dotnet run simulate validonly <word>");
            Console.WriteLine("dotnet run simulate maxeliminations <word>");
            Console.WriteLine("dotnet run simulate compare");
            Console.WriteLine("dotnet run bird");
            Console.WriteLine("dotnet run bird best <number>");
            Console.WriteLine("dotnet run bird worst <number>");
        }

        /// <summary>
        /// Display best or worst Wordle results.
        /// </summary>
        /// <param name="filePath">File path of list of words.</param>
        /// <param name="displayCount">Number of words to show.</param>
        /// <param name="showWorst">If true, shows the worst-scoring words instead of the best.
        /// </para>
        private static void BestOrWorst(string filePath, int displayCount, bool showWorst)
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize(filePath);
            string superlative = showWorst ? "lowest-scoring" : "highest-scoring";
            Console.WriteLine($"Computing the {superlative} starter words.");
            ImmutableList<(string, double)> topWordsOfAll =
                EliminationScoreSolver.GetTopScoringWords(
                    wordsCollection, displayCount, true, showWorst);
            displayTopWords(topWordsOfAll);
        }

        /// <summary>
        /// Display the Wordle results if the top-scoring valid word is taken each turn.
        /// </summary>
        /// <returns>The number of tries taken.</returns>
        private static int SimulateValidOnly(string solution, bool shouldPrint = true)
        {
            ConditionalPrint("Loading list of words.", shouldPrint);
            WordsCollection wordsCollection = WordsCollection.Initialize(WORDLE_FILE);

            if (!wordsCollection.allWords.Contains(solution))
            {
                ConditionalPrint($"{solution} not found in the Wordle dictionary.", shouldPrint);
                return -1;
            }

            int tries = 0;
            while (wordsCollection.currentWords.Count > 0)
            {
                string guess =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, 1, false)[0].Item1;
                GuessResult result = GuessResult.FromGuessAndSolution(guess, solution);
                ConditionalPrint(result.ToString(), shouldPrint);

                tries++;
                if (result.UserWon())
                {
                    break;
                }

                wordsCollection.EliminateWords(result);
            }

            ConditionalPrint($"Completed in {tries} tries.", shouldPrint);
            return tries;
        }

        /// <summary>
        /// Display the Wordle results if the top-scoring word, including invalid words, is chosen
        /// each time until only one option remains.
        /// </summary>
        /// <returns>The number of tries taken.</returns>
        private static int SimulateMaximizeEliminations(string solution, bool shouldPrint = true)
        {
            ConditionalPrint("Loading list of words.", shouldPrint);
            WordsCollection wordsCollection = WordsCollection.Initialize(WORDLE_FILE);

            if (!wordsCollection.allWords.Contains(solution))
            {
                ConditionalPrint($"{solution} not found in the Wordle dictionary.", shouldPrint);
                return -1;
            }

            int tries = 0;
            HashSet<string> wordsTried = new HashSet<string>();
            while (wordsCollection.currentWords.Count > 0)
            {
                ImmutableList<(string, double)> validGuesses =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, 2, false);
                ImmutableList<(string, double)> anyGuesses =
                    EliminationScoreSolver.GetTopScoringWords(wordsCollection, 1, true);

                String guess;
                if (validGuesses.Count == 1
                    || wordsTried.Contains(anyGuesses[0].Item1))
                {
                    guess = validGuesses[0].Item1;
                }
                else
                {
                    guess = anyGuesses[0].Item1;
                }
                
                GuessResult result = GuessResult.FromGuessAndSolution(guess, solution);
                ConditionalPrint(result.ToString(), shouldPrint);

                tries++;
                if (result.UserWon())
                {
                    break;
                }
                wordsTried.Add(guess);

                wordsCollection.EliminateWords(result);
            }

            ConditionalPrint($"Completed in {tries} tries.", shouldPrint);
            return tries;
        }

        /// <summary>
        /// Compare valid-only and max-eliminations algorithms across all words.
        /// </summary>
        private static void SimulateCompare()
        {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize(WORDLE_FILE);
            ImmutableHashSet<string> wordsSubset = wordsCollection.GetRandomSubset(500);

            int validOnlyScoreSum = 0;
            int validOnlyFailCount = 0;
            int maxEliminationsScoreSum = 0;
            int maxEliminationsFailCount = 0;
            int wordsSoFar = 0;
            int wordCount = wordsSubset.Count;

            Console.WriteLine("word\tvalid-only score\tmax-eliminations score\tprogress");
            foreach (string solution in wordsSubset)
            {
                int validOnlyScore = SimulateValidOnly(solution, false);
                validOnlyScoreSum += validOnlyScore;
                validOnlyFailCount += validOnlyScore > 6 ? 1 : 0;

                int maxEliminationsScore = SimulateMaximizeEliminations(solution, false);
                maxEliminationsScoreSum += maxEliminationsScore;
                maxEliminationsFailCount += maxEliminationsScore > 6 ? 1 : 0;

                wordsSoFar++;

                Console.WriteLine($"{solution}\t{validOnlyScore}\t{maxEliminationsScore}\t{wordsSoFar}/{wordCount}");
            }

            Console.WriteLine($"Valid only strategy:");
            Console.WriteLine($"\tAverage score: {(double)validOnlyScoreSum / wordCount}");
            Console.WriteLine($"\tNumber of failures (> 6 tries): {(double)validOnlyFailCount}/{wordCount}");
            Console.WriteLine($"Max eliminations strategy:");
            Console.WriteLine($"\tAverage score: {(double)maxEliminationsScoreSum / wordCount}");
            Console.WriteLine($"\tNumber of failures (> 6 tries): {(double)maxEliminationsFailCount}/{wordCount}");
        }

        /// <param name="filePath">File path of list of words.</param>
        private static void RunInteractive(string filePath) {
            Console.WriteLine("Loading list of words.");
            WordsCollection wordsCollection = WordsCollection.Initialize(filePath);
            
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

                int possibleWords = wordsCollection.currentWords.Count;
                if (possibleWords == 1)
                {
                    Console.WriteLine("There is 1 possible word.");
                }
                else
                {
                    Console.WriteLine(
                        $"There are {wordsCollection.currentWords.Count} possible words.");
                }

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
                string scoreStr = string.Format("{0:0.000}", pair.Item2);
                Console.WriteLine($"{i + 1}\t{pair.Item1}\t{scoreStr}");
            }
        }

        private static void ConditionalPrint(String message, bool shouldPrint)
        {
            if (shouldPrint)
            {
                Console.WriteLine(message);
            }
        }
    }
}
