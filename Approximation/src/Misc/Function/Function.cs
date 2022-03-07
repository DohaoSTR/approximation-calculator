using Mathos.Parser;
using System;
using System.Linq;

namespace Approximation
{
    public class Function : IFunction
    {
        private readonly string _function;

        private readonly char[] _variables = { 'x', 'y', 'z' };

        public Function(string functiuon)
        {
            _function = functiuon;
        }

        public double GetResult(double value)
        {
            return GetResult(value, "x");
        }

        public double GetResult(double value, string nameVariable)
        {
            MathParser mathParser = new MathParser();

            mathParser.LocalVariables.Add(nameVariable, value);
            double result = mathParser.Parse(_function);

            return result;
        }

        public override string ToString()
        {
            return _function;
        }

        public int GetDegree()
        {
            int degree = 1;
            int index = 0;

            foreach (char variable in _variables)
            {
                if (_function.Contains(variable + "^"))
                {
                    int currentDegree = Convert.ToInt32(_function[index + 1]);

                    if (currentDegree > degree)
                    {
                        degree = currentDegree;
                    }
                }

                index++;
            }

            return degree;
        }

        public int GetNumberCoefficients()
        {
            int countParameters = 1;

            foreach (char symbol in _function)
            {
                if (_variables.Contains(symbol))
                {
                    countParameters++;
                }
            }

            return countParameters;
        }
    }
}
