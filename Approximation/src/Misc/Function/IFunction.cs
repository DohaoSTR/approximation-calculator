namespace StandardMath
{
    public interface IFunction
    {
        double GetResult(double value);

        double GetResult(double value, string variableName);
    }
}
