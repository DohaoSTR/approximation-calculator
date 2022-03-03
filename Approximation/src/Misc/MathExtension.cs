namespace Approximation
{
    public static class MathExtension
    {
        public static double GetFactorial(double power)
        {
            if (power == 0)
            {
                return 1;
            }
            else
            {
                return power * GetFactorial(power - 1);
            }
        }
    }
}
