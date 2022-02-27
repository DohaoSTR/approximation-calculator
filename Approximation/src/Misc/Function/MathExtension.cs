using System;

namespace Approximation
{
    public static class MathExtension
    {
        public static int GetFactorial(int power)
        {
            if (power < 0)
            {
                throw new ArgumentOutOfRangeException("Степень должна быть больше либо равна нулю!");
            }
            else if (power == 0 || power == 1)
            {
                return power;
            }
            else
            {
                return power * GetFactorial(power - 1);
            }
        }
    }
}
