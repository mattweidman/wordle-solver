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
        /// Whether a guess is still possible after this guess result is shown.
        /// </summary>
        public bool Accepts(string other)
        {
            if (other.Length != this.letterColors.Count)
            {
                throw new ArgumentException("String is not the right number of characters.");
            }

            for (int i = 0; i < other.Length; i++)
            {
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

                    if (!other.Contains(this.guess[i]))
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

                    // Reject if the letter in the previous guess is gray, there is no other
                    // indication that the letter exists elsewhere, and the letter exists in other.
                    if (!this.hasNonGrayChar(this.guess[i]) && other.Contains(this.guess[i]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool hasNonGrayChar(char c)
        {
            for (int j = 0; j < this.guess.Length; j++)
            {
                if (this.guess[j] == c && this.letterColors[j] != LetterColor.GRAY)
                {
                    return true;
                }
            }

            return false;
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