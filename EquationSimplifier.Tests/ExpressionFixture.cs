using System;
using NUnit.Framework;

namespace SimplifyEquation.Tests
{
    [TestFixture]
    class ExpressionFixture
    {
        [Test]
        public void Parse_PositiveCoeff_OK()
        {
            var stringToParse = "2.9";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(2.9f, expr.Coefficient);
            Assert.AreEqual(string.Empty, expr.VariablesString);
        }

        [Test]
        public void Parse_NegativeCoeff_OK()
        {
            var stringToParse = "-1.3";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(-1.3f, expr.Coefficient);
            Assert.AreEqual(string.Empty, expr.VariablesString);
        }

        [Test]
        public void Parse_PositiveVariable_OK()
        {
            var stringToParse = "xy";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(1.0f, expr.Coefficient);
            Assert.AreEqual("xy", expr.VariablesString);
        }

        [Test]
        public void Parse_NegativeVariable_OK()
        {
            var stringToParse = "-z";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(-1.0f, expr.Coefficient);
            Assert.AreEqual("z", expr.VariablesString);
        }

        [Test]
        public void Parse_PositiveVariableWithExponent_OK()
        {
            var stringToParse = "x^-1";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(1.0f, expr.Coefficient);
            Assert.AreEqual("x^-1", expr.VariablesString);
        }

        [Test]
        public void Parse_NegativeVariableWithExponent_OK()
        {
            var stringToParse = "-y^777";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(-1.0f, expr.Coefficient);
            Assert.AreEqual("y^777", expr.VariablesString);
        }

        [Test]
        public void Parse_PositiveExpression_OK()
        {
            var stringToParse = "29.01x^89";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(29.01f, expr.Coefficient);
            Assert.AreEqual("x^89", expr.VariablesString);
        }

        [Test]
        public void Parse_ExpressionWithMultipleVariables_OK()
        {
            var stringToParse = "29.01x^19y^89";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(29.01f, expr.Coefficient);
            Assert.AreEqual("x^19y^89", expr.VariablesString);
        }

        [Test]
        public void Parse_NegativeExpression_OK()
        {
            var stringToParse = "-11.01y^2017";
            var expr = Expression.Parse(stringToParse);
            Assert.AreEqual(-11.01f, expr.Coefficient);
            Assert.AreEqual("y^2017", expr.VariablesString);
        }

        [TestCase("+")]
        [TestCase("-")]
        public void Parse_Sign_ThrowsArgumentException(string stringToParse)
        {
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }

        [Test]
        public void Parse_VariableWithExponentMissing_ThrowsArgumentException()
        {
            var stringToParse = "x^";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }
        
        [Test]
        public void Parse_VariableWithBaseMissing_ThrowsArgumentException()
        {
            var stringToParse = "^111";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }

        [Test]
        public void Parse_NegativeVariableWithBaseMissing_ThrowsArgumentException()
        {
            var stringToParse = "-^222";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }

        [Test]
        public void Parse_IncorrectExpression_ThrowsArgumentException()
        {
            var stringToParse = @"¯\_(ツ)_/¯";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }

        [Test]
        public void Parse_ExpressionWithoutExponent_ThrowsArgumentException()
        {
            var stringToParse = "1337z^";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }

        [Test]
        public void Parse_ExpressionWithoutBaseVariable_ThrowsArgumentException()
        {
            var stringToParse = "13^89";
            Assert.Throws<ArgumentException>(() => Expression.Parse(stringToParse));
        }
    }
}
