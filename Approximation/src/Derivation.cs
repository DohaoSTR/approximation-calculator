using System.Collections.Generic;

namespace Approximation
{
    public class Derivation
    {
        private readonly IReadOnlyList<Point> _points;

        public Derivation(IReadOnlyList<Point> points)
        {
            _points = points;
        }

        public IEnumerable<Point> NewtonPolynomial()
        {
            ICollection<Point> resultPoints = new List<Point>();

            double h = _points[1].X - _points[0].X;

            for (int i = 0; i < _points.Count; i++)
            {
                double q = (_points[i].X - _points[0].X) / h;

                double sum = GetEndDifference(0, 1);

                for (int j = 2; j < _points.Count; j++)
                {
                    sum += GetNumerator(q, j) * GetEndDifference(0, j) / MathExtension.GetFactorial(j);
                }

                resultPoints.Add(new Point(_points[i].X, sum / h));
            }
            return resultPoints;
        }

        private double GetEndDifference(int numY, int power)
        {
            double result;

            if (power == 1)
            {
                return _points[numY + 1].Y - _points[numY].Y;
            }
            else
            {
                result = GetEndDifference(numY + 1, power - 1) - GetEndDifference(numY, power - 1);

            }
            return result;
        }

        private double GetNumerator(double q, int countPoints)
        {
            if (countPoints == 2)
            {
                return 2 * q - 1;
            }
            else
            {
                double[] multipliers = new double[countPoints];

                for (int i = 0; i < countPoints; i++)
                {
                    multipliers[i] = q - i;
                }

                int[,] terms = FindAllTerms(multipliers.Length, multipliers.Length - 1);

                double sum = 0;
                for (int i = 0; i < terms.GetLength(0); i++)
                {
                    double m = 1;
                    for (int j = 0; j < terms.GetLength(1); j++)
                    {
                        m *= multipliers[terms[i, j]];
                    }
                    sum += m;
                }
                return sum;
            }
        }

        private int[,] FindAllTerms(int power, int m)
        {
            int[,] result = new int[power, m];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (i == 0)
                    {
                        result[i, j] = j;
                    }
                    else
                    {
                        if (m - i == j)
                        {
                            int k = result[0, j];
                            result[i, j] = ++k;
                        }
                        else
                        {
                            int k = result[i - 1, j];
                            result[i, j] = k;
                        }
                    }
                }
            }
            return result;
        }
    }
}
