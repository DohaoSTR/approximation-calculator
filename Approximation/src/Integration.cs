using System;

namespace StandardMath
{
    public class Integration
    {
        private readonly IFunction _function;

        public Integration(IFunction function)
        {
            _function = function;
        }

        /// <summary>
        /// Метод парабол (Симпсона)
        /// </summary>
        /// <remarks>
        /// Используется для интегрирования функций
        /// </remarks>
        /// <param name="interval">Отрезок интегрирования</param>
        /// <param name="partialIntervalsNumber">Количество элементарных отрезков интегрирования</param>
        /// <returns>Значение интеграла</returns>
        public double ParabolaMethod(Interval interval, double partialIntervalsNumber)
        {
            double step = (interval.End - interval.Start) / partialIntervalsNumber;

            double result = step / 3;

            double oddSum = 0;
            double evenSum = 0;

            int index = 1;

            for (double value = interval.Start; value <= interval.End; value += step)
            {
                if (index % 2 == 0 && index != partialIntervalsNumber && index != 0)
                {
                    oddSum += _function.GetResult(value);
                }
                else
                {
                    evenSum += _function.GetResult(value);
                }

                index++;
            }

            result *= _function.GetResult(interval.Start) + (4 * oddSum) + (2 * evenSum) + _function.GetResult(interval.End);

            return result;
        }

        /// <summary>
        /// Метод Гаусса
        /// Используется для интегрирования функций
        /// </summary>
        /// <param name="interval">Отрезок интегрирования</param>
        /// <param name="polynomialDegree">Cтепень многочлена или количество его корней</param>
        /// <returns>Значение интеграла</returns>
        public double GaussMethod(Interval interval, int polynomialDegree)
        {
            double result = 0;

            for (int index = 1; index <= polynomialDegree; index++)
            {
                double rootPolynomialLegenre = GetRootPolynomialLegendre(polynomialDegree - 1, index, polynomialDegree);

                double value = ((interval.Start + interval.End) / 2) + ((interval.End - interval.Start) / 2 * rootPolynomialLegenre);

                double polynomLegendre = 2 / (Math.Pow(GetDerivatPolynomialLegendre(rootPolynomialLegenre, polynomialDegree), 2) * (1 - Math.Pow(rootPolynomialLegenre, 2)));

                result += polynomLegendre * _function.GetResult(value);
            }
            result *= (interval.End - interval.Start) / 2;

            return result;
        }

        private double GetPolynomialLegendre(double rootPolynomialLegendre, int polynomIndex)
        {
            if (polynomIndex == 0)
            {
                return 1;
            }
            else if (polynomIndex == 1)
            {
                return rootPolynomialLegendre;
            }
            else
            {
                double r = (((2 * (polynomIndex - 1)) + 1) * rootPolynomialLegendre * GetPolynomialLegendre(rootPolynomialLegendre, polynomIndex - 1) /
                    (polynomIndex - 1 + 1)) - ((polynomIndex - 1) * GetPolynomialLegendre(rootPolynomialLegendre, polynomIndex - 2) / polynomIndex);
                return r;
            }
        }

        private double GetRootPolynomialLegendre(int k, int index, int polynomIndex)
        {
            if (k == 0)
            {
                return Math.Cos(Math.PI * ((4 * index) - 1) / ((4 * polynomIndex) + 2));
            }
            else
            {
                double t = GetRootPolynomialLegendre(k - 1, index, polynomIndex);

                return t - (GetPolynomialLegendre(t, polynomIndex) / GetDerivatPolynomialLegendre(t, polynomIndex));
            }
        }

        private double GetDerivatPolynomialLegendre(double rootPolynomialLegendre, int polynomIndex)
        {
            double p1 = GetPolynomialLegendre(rootPolynomialLegendre, polynomIndex - 1);
            double p2 = GetPolynomialLegendre(rootPolynomialLegendre, polynomIndex);

            return polynomIndex * (p1 - (p2 * rootPolynomialLegendre)) / (1 - Math.Pow(rootPolynomialLegendre, 2));
        }
    }
}
