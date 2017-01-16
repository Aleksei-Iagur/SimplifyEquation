using System;
using System.Linq;

namespace SimplifyEquation
{
    public class Expression
    {
        public float Coefficient { get; set; }
        public string VariablesString { get; set; }

        public static Expression Parse(string str)
        {
            str = str.Replace('.', ',');
            
            var sign = GetSign(str);
            var coeff = sign == Sign.Plus ? GetUnsignedCoeff(str) : -GetUnsignedCoeff(str);
            var varString = GetVariablesString(str);

            return new Expression { Coefficient = coeff, VariablesString = varString };
        }
        
        private static Sign GetSign(string input)
        {
            bool positiveSign = true;

            if (IsSign(input[0]))
            {
                positiveSign = input[0] == '+';
            }

            return positiveSign ? Sign.Plus : Sign.Minus;
        }

        private static bool IsSign(char c)
        {
            return c == '-' || c == '+';
        }

        private static float GetUnsignedCoeff(string input)
        {
            int firstLetterIndex = GetFirstLetterIndex(input);

            if (firstLetterIndex == 0 ||
                (firstLetterIndex == 1 && IsSign(input[0])))
            {
                return 1.0f;
            }

            var coeffString = firstLetterIndex == -1 ? input : input.Substring(0, firstLetterIndex);

            float coeff;
            bool parsed = float.TryParse(coeffString, out coeff);
            if (!parsed)
            {
                throw new ArgumentException();
            }

            return Math.Abs(coeff);
        }

        private static string GetVariablesString(string input)
        {
            int firstLetterIndex = GetFirstLetterIndex(input);

            if (firstLetterIndex == -1)
            {
                return String.Empty;
            }

            var varString = input.Substring(firstLetterIndex);
            if (!IsValidVarString(varString))
            {
                throw new ArgumentException();
            }

            return varString;
        }

        private static bool IsValidVarString(string str)
        {
            var firstExponentIndex = str.IndexOf('^');
            if (firstExponentIndex == -1 && str.All(Char.IsLetter))
            {
                return true;
            }

            if (firstExponentIndex == 0 || firstExponentIndex == str.Length - 1)
            {
                return false;
            }

            for (int i = 1; i < str.Length - 1; i++)
            {
                if (str[i] == '^')
                {
                    if (!IsBaseALetter(str, i) || !IsExponentANumber(str, i))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static bool IsExponentANumber(string str, int caretIndex)
        {
            return Char.IsDigit(IsSign(str[caretIndex + 1]) ? str[caretIndex + 2] : str[caretIndex + 1]);
        }

        private static bool IsBaseALetter(string str, int caretIndex)
        {
            return Char.IsLetter(str[caretIndex - 1]);
        }

        private static int GetFirstLetterIndex(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]))
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
