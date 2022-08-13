# wordle-solver
Attempt to make a program that can solve Wordle better than I can.

## Algorithm

Currently, this solver works by assigning each possible word an "elimination score." This score is approximately the expected number of possible words that would be eliminated if that word were chosen. Each time you play a word, possible words are eliminated until you get to the answer.

This is a greedy algorithm, so there are a few cases where guessing the best-scoring answer actually hurts your chances in the long run. An example word where this algorithm does poorly is "nares." Another example is "hater". See the TODOs. However, for most words, this algorithm does good enough.

## TODO

* Don't give extra points to words that don't eliminate more words.
  * Example: Guess "glade"; result "ggggr". Two options: "glady" or "glads". "slays" gives a score of 0.5, and "glady" gives a score of 0.2 even though they both eliminate the same number of words (1). Ideally we would want to play "glady" rather than "slays" because "glady" has a chance of being the actual answer.

## How to run

Run: `dotnet run --project src`

Run unit tests: `dotnet test`

If you are running the script multiple times and don't want to wait to build each time, you can build once and run using the following steps:
1. From the root directory, run `dotnet build`.
2. Run the executable file in src/bin/Debug/net5.0.

PC: `.\src\bin\Debug\net5.0\wordle-solver.exe`

Mac: `./src/bin/Debug/net5./wordle-solver`