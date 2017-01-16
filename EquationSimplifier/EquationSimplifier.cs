using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimplifyEquation
{
    public class EquationSimplifier : IEquationSimplifier
    {
        public string Simplify(string equation)
        {
            if (!IsValid(equation))
            {
                throw new ArgumentException("Incorrect equation.");
            }
            Dictionary<string, float> expressions = new Dictionary<string, float>();


            equation = equation.Replace(" ", "");

            var modificatorIsPositive = true;
            var modificators = new Stack<Sign>();
            var expressionStart = 0;

            for (var i = 1; i < equation.Length; i++)
            {
                var currentSymbol = equation[i];
                if (expressionStart == i)
                {
                    UpdateModificatorIfNeeded(currentSymbol, modificators, ref modificatorIsPositive);
                    continue;
                }

                if (ExpressionIsEnded(equation, i))
                {
                    var rawExpression = equation.Substring(expressionStart, i - expressionStart);

                    expressionStart = GetNextExpressionStartIndex(equation, i);

                    if (currentSymbol == '(')
                    {
                        {
                            var modificator = GetModificatorSign(equation, i);
                            modificators.Push(modificator);
                            if (modificator == Sign.Minus)
                            {
                                modificatorIsPositive = !modificatorIsPositive;
                            }

                            continue;
                        }
                    }

                    if (rawExpression.Equals("="))
                    {
                        continue;
                    }

                    AddSymbolToDic(rawExpression, expressions, modificatorIsPositive);

                    UpdateModificatorIfNeeded(currentSymbol, modificators, ref modificatorIsPositive);
                }
            }
            
            if(expressionStart < equation.Length)
            {
                AddSymbolToDic(equation.Substring(expressionStart), expressions, modificatorIsPositive);
            }

            return MakeAnEquationString(expressions);
        }

        private bool IsValid(string equation)
        {
            return equation.Count(c => c == '=') == 1 &&
                   equation.Length >= 3 &&
                   CorrectNumberOfBrackets(equation);
        }

        private bool CorrectNumberOfBrackets(string equation)
        {
            var count = 0;
            for (var i = 0; i < equation.Length; i++)
            {
                switch (equation[i])
                {
                    case '(':
                        count++;
                        break;
                    case ')':
                        count--;
                        break;
                }
            }

            return count == 0;
        }

        private void UpdateModificatorIfNeeded(char currentSymbol, Stack<Sign> modificators, ref bool modificatorIsPositive)
        {
            switch (currentSymbol)
            {
                case '=':
                    if (modificators.Count > 0)
                    {
                        throw new ArgumentException("Left part of equation contains unclosed brackets.");
                    }

                    modificatorIsPositive = !modificatorIsPositive;
                    break;
                case ')':
                    {
                        var modificator = modificators.Pop();
                        if (modificator == Sign.Minus)
                        {
                            modificatorIsPositive = !modificatorIsPositive;
                        }
                        break;
                    }
            }
        }

        private Sign GetModificatorSign(string equation, int i)
        {
            if (i == 0)
            {
                return Sign.Plus;
            }

            var previousSymbol = equation[i - 1];

            switch (previousSymbol)
            {
                case '=':
                case '+':
                    return Sign.Plus;
                case '-':
                    return Sign.Minus;
                default:
                    throw new ArgumentException($"Incorrect sign before openning bracket: '{i-1}'.");
            }
        }

        private int GetNextExpressionStartIndex(string equation, int i)
        {
            int index;
            var symbol = equation[i];
            switch (symbol)
            {
                case '=':
                case '(':
                case ')':
                    index = i + 1;
                    break;
                case '+':
                case '-':
                    index = i;
                    break;
                default:
                    throw new NotImplementedException($"Operator {equation[i]} is not implemented.");
            }

            if (index == equation.Length && symbol != ')')
            {
                throw new ArgumentException("Incorrect equation.");
            }

            return index;
        }

        private bool ExpressionIsEnded(string equation, int i)
        {
            char c = equation[i];
            return c == '+' || IsCoefficientMinus(equation, i) || c == '=' || c == '(' || c == ')';
        }

        private bool IsCoefficientMinus(string equation, int i)
        {
            return equation[i] == '-' && i > 0 && equation[i-1] != '^';
        }
        
        private string MakeAnEquationString(Dictionary<string, float> expressions)
        {
            if (expressions.Count == 0)
            {
                throw new ArgumentException("No expressions parsed.");
            }

            var emptyEquation = "0 = 0";
            if (expressions.All(expr => String.IsNullOrEmpty(expr.Key)))
            {
                var sum = expressions.Sum(expression => expression.Value);
                if (sum.CompareTo(0.0f) != 0)
                {
                    throw new ArgumentException("Incorrect equation.");
                }

                return emptyEquation;
            }
            
            var sb = new StringBuilder();
            var addedElements = 0;
            foreach (var el in expressions)
            {
                var sign = el.Value >= 0 ? '+' : '-';
                var coeff = CoeffIsNotNeeded(el) ? string.Empty : Math.Abs(el.Value).ToString(CultureInfo.InvariantCulture);

                if (coeff.Equals("0"))
                {
                    continue;
                }

                sb.Append($"{sign} {coeff}{el.Key} ");
                addedElements++;
            }

            if (addedElements == 0)
            {
                return emptyEquation;
            }

            TrimStartingPlusIfNeeded(sb);

            sb.Append("= 0");

            return sb.ToString();
        }

        private static bool CoeffIsNotNeeded(KeyValuePair<string, float> el)
        {
            return Math.Abs(el.Value).CompareTo(1.0f) == 0 && !String.IsNullOrEmpty(el.Key);
        }

        private static void TrimStartingPlusIfNeeded(StringBuilder sb)
        {
            if (sb[0] == '+')
            {
                sb.Remove(0, 2);
            }
        }

        private void AddSymbolToDic(string input, IDictionary<string, float> dic, bool positiveModificator)
        {
            input = input.TrimStart('=');
            var symbol = Expression.Parse(input);
            var coeff = positiveModificator ? symbol.Coefficient : -symbol.Coefficient;
            string key = symbol.VariablesString;
            
            if (dic.ContainsKey(key))
            {
                dic[key] += coeff;
            }
            else
            {
                dic.Add(key, coeff);
            }
        }
    }
}
