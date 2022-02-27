using Mathos.Parser;

namespace Approximation
{
    public class Function : IFunction
    {
        private readonly string _function;

        public Function(string functiuon)
        {
            _function = functiuon;
        }

        public double GetResult(double value)
        {
            MathParser mathParser = new MathParser();

            mathParser.LocalVariables.Add("x", value);
            double result = mathParser.Parse(_function);
            mathParser.LocalVariables.Clear();

            return result;
        }

        public double GetResult(double value, string nameVariable)
        {
            MathParser mathParser = new MathParser();

            mathParser.LocalVariables.Add(nameVariable, value);
            double result = mathParser.Parse(_function);
            mathParser.LocalVariables.Clear();

            return result;
        }

        public override string ToString()
        {
            return _function;
        }
    }
}
