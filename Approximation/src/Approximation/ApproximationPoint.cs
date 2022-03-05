using LinearAlgebra;
using LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Approximation
{
    public class ApproximationPoint
    {
        private readonly IReadOnlyList<Point> _points;

        public ApproximationPoint(IEnumerable<Point> points)
        {
            if (points != null)
            {
                _points = (IReadOnlyList<Point>)points;
            }
            else
            {
                throw new NullReferenceException("Список points не должен быть равен null!");
            }
        }

        public IEnumerable<Point> MethodLinearInterpolation(double step)
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (int index = 0; index < _points.Count; index++)
            {
                if (index == _points.Count - 1)
                {
                    break;
                }
                else
                {
                    for (double x = _points[index].X; x <= _points[index + 1].X; x += step)
                    {
                        double y = _points[index].Y + ((_points[index + 1].Y - _points[index].Y) /
                            (_points[index + 1].X - _points[index].X) * (x - _points[index].X));

                        resultPoints.Add(new Point(x, y));
                    }
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> MethodSquareInterpolation(double step)
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (int k = 0; k < _points.Count - 2; k++)
            {
                double[,] coefficientsOfEquation = new double[3, 3];
                double[] freeCoefficients = new double[3];

                for (int i = 0; i < coefficientsOfEquation.GetLength(0); i++)
                {
                    for (int j = 0; j < coefficientsOfEquation.GetLength(1); j++)
                    {
                        coefficientsOfEquation[i, j] = Math.Pow(_points[k + i].X, 2 - j);
                    }

                    freeCoefficients[i] = _points[k + i].Y;
                }

                MatrixStorage<double> storageFirstCoefficients = new MatrixStorage<double>(coefficientsOfEquation);
                Matrix matrixSystemOfEquations = new Matrix(storageFirstCoefficients);

                double[] roots = Matrix.GaussJordan(matrixSystemOfEquations, freeCoefficients);

                if (k == 0)
                {
                    for (double x = _points[0].X; x < _points[1].X; x += step)
                    {
                        double y = (roots[0] * Math.Pow(x, 2)) + (roots[1] * x) + roots[2];

                        resultPoints.Add(new Point(x, y));
                    }
                }

                for (double x = _points[k + 1].X; x <= _points[k + 2].X; x += step)
                {
                    double y = (roots[0] * Math.Pow(x, 2)) + (roots[1] * x) + roots[2];

                    resultPoints.Add(new Point(x, y));
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> MethodCubicInterpolation(double step)
        {
            ICollection<Point> resultPoints = new List<Point>();

            Dictionary<double, double> known = new Dictionary<double, double>();

            foreach (Point point in _points)
            {
                known.Add(point.X, point.Y);
            }

            SplineInterpolator scaler = new SplineInterpolator(known);

            double start = known.First().Key;
            double end = known.Last().Key;

            for (double x = start; x <= end; x += step)
            {
                double y = scaler.GetValue(x);

                if (y > -1e10 && y < 1e10)
                {
                    resultPoints.Add(new Point(x, y));
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> LagrandePolynomial(double step)
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = _points[0].X; x <= _points[_points.Count - 1].X; x += step)
            {
                double sum = 0;

                for (int i = 0; i < _points.Count; i++)
                {
                    double multiplication = 1;

                    for (int j = 0; j < _points.Count; j++)
                    {
                        if (j != i)
                        {
                            multiplication *= (x - _points[j].X) / (_points[i].X - _points[j].X);
                        }
                    }

                    sum += _points[i].Y * multiplication;
                }

                resultPoints.Add(new Point(x, sum));
            }

            return resultPoints;
        }

        public IEnumerable<Point> NewtonPolynomial(double step)
        {
            ICollection<Point> newPoints = new List<Point>();

            double h = _points[1].X - _points[0].X;

            for (double x = _points[0].X; x <= _points[_points.Count - 1].X; x += step)
            {
                double px = _points[0].Y;
                double q = Math.Round((x - _points[0].X) / h, 3);

                for (int i = 1; i < _points.Count; i++)
                {
                    double endDiff = FindEndDifference(0, i);

                    double numerator = 1;
                    for (int j = 0; j < i; j++)
                    {
                        numerator *= q - j;
                    }

                    px += numerator * endDiff / MathExtension.GetFactorial(i);
                }

                newPoints.Add(new Point(x, px));
            }

            return newPoints;
        }

        public IEnumerable<Point> LeastSquareMethod(double step, int power)
        {
            ICollection<Point> resultPoints = new List<Point>();

            double[] values = new double[_points.Count];
            double[] keys = new double[_points.Count];

            for (int i = 0; i < _points.Count; i++)
            {
                values[i] = _points[i].X;
                keys[i] = _points[i].Y;
            }

            int size = power + 1;

            double[,] coefficientsOfEquation = new double[size, size];
            coefficientsOfEquation[0, 0] = _points.Count;

            double[] freeCoefficients = new double[size];
            freeCoefficients[0] = SumArrayElements(keys, 1);

            for (int i = 1; i < size; i++)
            {
                coefficientsOfEquation[0, i] = SumArrayElements(values, i);
                freeCoefficients[i] = SumArrayElements(values, keys, i);
            }

            for (int i = 1; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    coefficientsOfEquation[i, j] = SumArrayElements(values, j + i);
                }
            }

            MatrixStorage<double> storageFirstCoefficients = new MatrixStorage<double>(coefficientsOfEquation);
            Matrix matrixSystemOfEquations = new Matrix(storageFirstCoefficients);

            double[] roots = Matrix.GaussJordan(matrixSystemOfEquations, freeCoefficients);

            for (double x = _points[0].X; x <= _points[_points.Count - 1].X; x += step)
            {
                double y = 0;

                for (int i = 0; i < roots.Length; i++)
                {
                    y += roots[i] * Math.Pow(x, i);
                }

                resultPoints.Add(new Point(x, y));
            }

            return resultPoints;
        }

        private double FindEndDifference(int indexY, int power)
        {
            return power == 1
                ? _points[indexY + 1].Y - _points[indexY].Y
                : FindEndDifference(indexY + 1, power - 1) - FindEndDifference(indexY, power - 1);
        }

        private double SumArrayElements(double[] array, int power)
        {
            double sum = 0;

            for (int i = 0; i < array.Length; i++)
            {
                sum += Math.Pow(array[i], power);
            }

            return sum;
        }

        private double SumArrayElements(double[] firstArray, double[] secondArray, int power)
        {
            double sum = 0;

            for (int i = 0; i < firstArray.Length; i++)
            {
                sum += Math.Pow(firstArray[i], power) * secondArray[i];
            }

            return sum;
        }
    }
}
