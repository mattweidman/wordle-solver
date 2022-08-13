using NUnit.Framework;
using WordleSolver;

namespace WordleSolver.Tests
{
    public class GuessResultTests
    {
        [Test]
        public void Accepts_GrayRejectsSingleChar()
        {
            GuessResult result = GuessResult.FromString("raise", "rrrrr");
            Assert.IsFalse(result.Accepts("flack"));
        }

        [Test]
        public void Accepts_GrayRejectsMatchingChar()
        {
            GuessResult result = GuessResult.FromString("raise", "rrrrr");
            Assert.IsFalse(result.Accepts("batch"));
        }

        [Test]
        public void Accepts_YellowAcceptsSingleChar()
        {
            GuessResult result = GuessResult.FromString("raise", "ryrrr");
            Assert.IsTrue(result.Accepts("flack"));
        }

        [Test]
        public void Accepts_YellowRejectsMatchingChar()
        {
            GuessResult result = GuessResult.FromString("raise", "ryrrr");
            Assert.IsFalse(result.Accepts("batch"));
        }

        [Test]
        public void Accepts_YellowGrayAcceptsSingleChar()
        {
            GuessResult result = GuessResult.FromString("areae", "ygrrg");
            Assert.IsTrue(result.Accepts("crake"));
        }

        [Test]
        public void Accepts_YellowAcceptsDoubleChar()
        {
            GuessResult result = GuessResult.FromString("raise", "rrrry");
            Assert.IsTrue(result.Accepts("fleet"));
        }

        [Test]
        public void Accepts_DoubleYellowAcceptsDoubleChar()
        {
            GuessResult result = GuessResult.FromString("event", "yryrg");
            Assert.IsTrue(result.Accepts("beret"));
        }

        [Test]
        public void Accepts_DoubleYellowRejectsSingleChar()
        {
            GuessResult result = GuessResult.FromString("event", "yryrr");
            Assert.IsFalse(result.Accepts("raise"));
        }

        [Test]
        public void Accepts_GreenYellowRejectsSingleChar()
        {
            GuessResult result = GuessResult.FromString("yedes", "ryrgg");
            Assert.IsFalse(result.Accepts("puces"));
        }

        [Test]
        public void Accepts_GreenYellowAcceptsDoubleChar()
        {
            GuessResult result = GuessResult.FromString("guess", "rrrgy");
            Assert.IsTrue(result.Accepts("swish"));
        }

        [Test]
        public void Accepts_GreenAcceptsMatchingChar()
        {
            GuessResult result = GuessResult.FromString("glaze", "rrgrr");
            Assert.IsTrue(result.Accepts("track"));
        }

        [Test]
        public void Accepts_GreenAcceptsDoubleChar()
        {
            GuessResult result = GuessResult.FromString("glaze", "rggrr");
            Assert.IsTrue(result.Accepts("llama"));
        }
    }
}