using NUnit.Framework;
using WordleSolver;

namespace WordleSolver.Tests
{
    public class GuessResultTests
    {
        [Test]
        public void Accepts_RepeatedChars()
        {
            GuessResult result = GuessResult.FromString("areae", "ygrrg");
            Assert.IsTrue(result.Accepts("crake"));
        }
    }
}