# wordle-solver
Attempt to make a program that can solve Wordle better than I can.

## Algorithm

Currently, this solver works by assigning each possible word an "elimination score." This score is approximately the expected number of possible words that would be eliminated if that word were chosen. Each time you play a word, possible words are eliminated until you get to the answer.

This is a greedy algorithm, so there are a few cases where guessing the best-scoring answer actually hurts your chances in the long run. An example word where this algorithm does poorly is "nares." Another example is "hater". See the TODOs. However, for most words, this algorithm does good enough.

## TODOs

* If there are duplicate letters, and one duplicate is green and one is yellow, then words
where the char doesn't appear twice should be eliminated.
    * Example word: esses. Guessing "yedes" should eliminate "puces" because the "e" appears twice.
* Choose words that avoid "trapping" the user in a situation where there are a lot of green letters but still a lot of possible words.
    * Example word: nares. If you start with "soare", you will quickly get the last 4 letters, but there are over a dozen words that end with "ares". It takes 11 tries to get to "nares" because there we can only get info from one letter at a time.
* Create a simmulation framework to evaluate the performance of different algorithms.

## How to run

1. From the root directory, run `dotnet build`.
2. Run the executable file in src/bin/Debug/net5.0.

PC: `.\src\bin\Debug\net5.0\wordle-solver.exe`

Mac: `./src/bin/Debug/net5./wordle-solver`