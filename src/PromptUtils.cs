using System;
using System.Linq;

namespace WordleSolver
{
    public static class PromptUtils
    {
        /// <summary>
        /// Get a guess from the user. The guess needs to be 5 characters and can only use
        /// lower-case alphabetical characters. Keeps asking until the user enters a valid guess.
        /// </summary>
        public static string GetGuessFromUser()
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (input.Length != 5)
                {
                    Console.WriteLine("Must be 5 characters.");
                    continue;
                }

                if (!input.All(c => c >= 'a' && c <= 'z'))
                {
                    Console.WriteLine("Please only use lowercase alphabetical characters.");
                    continue;
                }

                return input;
            }
        }

        /// <summary>
        /// Get a result from the user. The result should be made up of "g", "y", and "r"
        /// characters, which indicate green, yellow, and red spaces respectively.
        /// </summary>
        public static GuessResult GetResultFromUser(string guess)
        {
            while (true)
            {
                string input = Console.ReadLine();

                if (input.Length != 5)
                {
                    Console.WriteLine("Must be 5 characters.");
                    continue;
                }

                if (!input.All(c => c == 'g' || c == 'y' || c == 'r'))
                {
                    Console.WriteLine("Please only enter 'g', 'y', or 'r'.");
                    continue;
                }

                return GuessResult.FromString(guess, input);
            }
        }
    }
}