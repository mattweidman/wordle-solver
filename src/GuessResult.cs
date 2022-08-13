using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace WordleSolver
{
    /// <summary>
    /// Represents the result from a single Wordle guess.
    /// </summary>
    public class GuessResult
    {
        private readonly ImmutableList<LetterColor> letterColors;

        private readonly string guess;

        public GuessResult(String guess, IEnumerable<LetterColor> letterColors)
        {
            this.guess = guess;
            this.letterColors = letterColors.ToImmutableList();
        }

        public static GuessResult FromGuessAndSolution(string guess, string solution)
        {
            if (guess.Length != solution.Length)
            {
                throw new ArgumentException("Guess and solution must have the same length.");
            }

            ImmutableList<LetterColor> letterColors = guess.Select((guessC, i) =>
            {
                if (guessC == solution[i])
                {
                    return LetterColor.GREEN;
                }
                else if (solution.Contains(guessC))
                {
                    return LetterColor.YELLOW;
                }
                else
                {
                    return LetterColor.GRAY;
                }
            }).ToImmutableList();

            return new GuessResult(guess, letterColors);
        }

        /// <summary>Compute from a string of "g", "y", and "r', which indicate green,
        /// yellow, and gray spaces.</summary>
        public static GuessResult FromString(string guess, string resultStr)
        {
            Dictionary<char, LetterColor> colorMap = new Dictionary<char, LetterColor>()
            {
                {'g', LetterColor.GREEN},
                {'y', LetterColor.YELLOW},
                {'r', LetterColor.GRAY},
            };

            return new GuessResult(guess, resultStr.Select(c => colorMap[c]));
        }

        /// <summary>
        /// Written as "guess\tgyr" where "gyr" is the 5-letter combination of "g", "y", and "r"
        /// that represent green, yellow, and gray spaces.
        /// </summary>
        public override string ToString()
        {
            Dictionary<LetterColor, char> colorMap = new Dictionary<LetterColor, char>()
            {
                {LetterColor.GREEN, 'g'},
                {LetterColor.YELLOW, 'y'},
                {LetterColor.GRAY, 'r'},
            };

            string gyr = string.Join("", this.letterColors.Select(color => colorMap[color]));
            return $"{guess}\t{gyr}";
        }

        /// <summary>
        /// Whether a guess is still possible after this guess result is shown.
        /// </summary>
        public bool Accepts(string other)
        {
            if (other.Length != this.letterColors.Count)
            {
                throw new ArgumentException("String is not the right number of characters.");
            }

            int[] guessCharCounts = new int[26];
            int[] otherCharCounts = new int[26];
            bool[] grayChars = new bool[26];

            for (int i = 0; i < other.Length; i++)
            {
                // Eliminate letters that don't match in the exact same position.
                if (this.letterColors[i] == LetterColor.GREEN)
                {
                    if (this.guess[i] != other[i])
                    {
                        return false;
                    }
                }
                else if (this.letterColors[i] == LetterColor.YELLOW)
                {
                    if (this.guess[i] == other[i])
                    {
                        return false;
                    }
                }
                else
                {
                    if (other[i] == this.guess[i])
                    {
                        return false;
                    }
                }

                // Count the characters and record whether each character is gray anywhere.
                int guessCharIndex = (int) (this.guess[i] - 'a');
                if (this.letterColors[i] == LetterColor.GRAY)
                {
                    grayChars[guessCharIndex] = true;
                }
                else
                {
                    guessCharCounts[guessCharIndex]++;
                }

                int otherCharIndex = (int) (other[i] - 'a');
                otherCharCounts[otherCharIndex]++;
            }

            // Eliminate words with the wrong number of characters.
            for (int i = 0; i < other.Length; i++)
            {
                int charIndex = (int) (this.guess[i] - 'a');
                
                if (grayChars[charIndex])
                {
                    // If a character appears gray anywhere, guess and other must contain it
                    // the exact same number of times.
                    if (guessCharCounts[charIndex] != otherCharCounts[charIndex])
                    {
                        return false;
                    }
                }
                else
                {
                    // If the character is not gray anywhere, other must contain the character
                    // at least as many times as guess.
                    if (guessCharCounts[charIndex] > otherCharCounts[charIndex])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool UserWon()
        {
            return this.letterColors.All(color => color == LetterColor.GREEN);
        }

        public enum LetterColor
        {
            GRAY,
            YELLOW,
            GREEN
        }
    }
}