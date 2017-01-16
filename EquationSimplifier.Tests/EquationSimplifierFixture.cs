using System;
using NUnit.Framework;

namespace SimplifyEquation.Tests
{
    [TestFixture]
    public class EquationSimplifierFixture
    {
        [TestCase("3 + 5x = -6", Result = "9 + 5x = 0")]
        [TestCase("x^2 + 3.5xy + y = y^2 - xy + y", Result = "x^2 + 4.5xy - y^2 = 0")]
        [TestCase("3 - 3 + 5x - 5x = 7 - 7", Result = "0 = 0")]
        [TestCase("0 = 0", Result = "0 = 0")]
        [TestCase("5z = 5z", Result = "0 = 0")]
        [TestCase("2x^7 + 3 - x^7 = -7", Result = "x^7 + 10 = 0")]
        [TestCase("3 + 5x^-9 = -6", Result = "9 + 5x^-9 = 0")]
        public string Simplify_SimpleEquation_OK(string input)
        {
            var sut = new EquationSimplifier();

            return sut.Simplify(input);
        }

        [TestCase("")]
        [TestCase("=")]
        [TestCase("49 = 0")]
        [TestCase("z^7 = ")]
        [TestCase(" = 10")]
        [TestCase("3 - (-5x + xy) + 2 - 6")]
        [ExpectedException(typeof(ArgumentException))]
        public void Simplify_IncorrectEquation_ThrowsArgumentException(string input)
        {
            var sut = new EquationSimplifier();
            sut.Simplify(input);
        }

        [TestCase("5z^x = 5z")]
        [ExpectedException(typeof(ArgumentException))]
        public void Simplify_IncorrectExponent_ThrowsArgumentException(string input)
        {
            var sut = new EquationSimplifier();
            sut.Simplify(input);
        }

        [TestCase("-(4x) = 7", Result = "- 4x - 7 = 0")]
        [TestCase("3 + (5x + 2) = -6", Result = "11 + 5x = 0")]
        [TestCase("3 - (5x + 2) = -6", Result = "7 - 5x = 0")]
        [TestCase("- (5x + 2) = -6", Result = "- 5x + 4 = 0")]
        [TestCase("5x = -(7 - 49)", Result = "5x - 42 = 0")]
        [TestCase("3 - (-(-5x + xy) + 2) = -6", Result = "7 - 5x + xy = 0")]
        public string Simplify_CorrectBrackets_OK(string input)
        {
            var sut = new EquationSimplifier();
            return sut.Simplify(input);
        }

        [TestCase("-3 (5x^2 - 55) = 8")]
        [TestCase("3 - (-5x + xy) + 2) = -6")]
        [TestCase("3 - (-5x + xy + 2 = -6")]
        [TestCase("3 - (-5x + xy + 2 = -6) + 7z")]
        [ExpectedException(typeof(ArgumentException))]
        public void Simplify_IncorrectBrackets_ThrowsArgumentException(string input)
        {
            var sut = new EquationSimplifier();
            sut.Simplify(input);
        }
    }
}
