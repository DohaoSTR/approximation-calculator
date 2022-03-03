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
    }
}
