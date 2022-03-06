using LinearAlgebra;
using LinearAlgebra.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Approximation
{
    public class ApproximationPoint
    {
        private readonly DiscreteFunction _discreteFunction;

        public double Step { get; private set; }

        public ApproximationPoint(IEnumerable<Point> points, double step)
        {
            if (points == null)
            {
                throw new NullReferenceException("Список points не должен быть равен null!");
            }

            if (points.Count() < 2)
            {
                throw new ArgumentOutOfRangeException("Необходимо минимум две точки в дискретной функции!");
            }

            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("Шаг должен быть больше нуля!");
            }

            DiscreteFunction discreteFunction = new DiscreteFunction(points);
            _discreteFunction = discreteFunction;

            discreteFunction.SortValueMax();

            Step = step;
        }

        public ApproximationPoint(DiscreteFunction discreteFunction, double step)
        {
            if (discreteFunction == null)
            {
                throw new NullReferenceException("Дискретная функция не может быть равна null!");
            }

            if (discreteFunction.Count < 2)
            {
                throw new ArgumentOutOfRangeException("Необходимо минимум две точки в дискретной функции!");
            }

            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("Шаг должен быть больше нуля!");
            }

            _discreteFunction = discreteFunction;
            _discreteFunction.SortValueMax();

            Step = step;
        }

        public IEnumerable<Point> MethodLinearInterpolation()
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (int index = 0; index < _discreteFunction.Count; index++)
            {
                if (index == _discreteFunction.Count - 1)
                {
                    break;
                }
                else
                {
                    for (double x = _discreteFunction[index].X; x <= _discreteFunction[index + 1].X; x += Step)
                    {
                        double y = _discreteFunction[index].Y + ((_discreteFunction[index + 1].Y - _discreteFunction[index].Y) /
                            (_discreteFunction[index + 1].X - _discreteFunction[index].X) * (x - _discreteFunction[index].X));

                        resultPoints.Add(new Point(x, y));
                    }
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> MethodSquareInterpolation()
        {
            if (_discreteFunction.Count < 3)
            {
                throw new ArgumentOutOfRangeException("Для метода квадратной интерполяции необходимо минимум три точки!");
            }

            ICollection<Point> resultPoints = new List<Point>();

            for (int k = 0; k < _discreteFunction.Count - 2; k++)
            {
                double[,] coefficientsOfEquation = new double[3, 3];
                double[] freeCoefficients = new double[3];

                for (int i = 0; i < coefficientsOfEquation.GetLength(0); i++)
                {
                    for (int j = 0; j < coefficientsOfEquation.GetLength(1); j++)
                    {
                        coefficientsOfEquation[i, j] = Math.Pow(_discreteFunction[k + i].X, 2 - j);
                    }

                    freeCoefficients[i] = _discreteFunction[k + i].Y;
                }

                MatrixStorage<double> storageFirstCoefficients = new MatrixStorage<double>(coefficientsOfEquation);
                Matrix matrixSystemOfEquations = new Matrix(storageFirstCoefficients);

                double[] roots = Matrix.GaussJordan(matrixSystemOfEquations, freeCoefficients);

                if (k == 0)
                {
                    for (double x = _discreteFunction[0].X; x < _discreteFunction[1].X; x += Step)
                    {
                        double y = (roots[0] * Math.Pow(x, 2)) + (roots[1] * x) + roots[2];

                        resultPoints.Add(new Point(x, y));
                    }
                }

                for (double x = _discreteFunction[k + 1].X; x <= _discreteFunction[k + 2].X; x += Step)
                {
                    double y = (roots[0] * Math.Pow(x, 2)) + (roots[1] * x) + roots[2];

                    resultPoints.Add(new Point(x, y));
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> MethodCubicInterpolation()
        {
            ICollection<Point> resultPoints = new List<Point>();

            Dictionary<double, double> known = new Dictionary<double, double>();

            foreach (Point point in _discreteFunction)
            {
                known.Add(point.X, point.Y);
            }

            SplineInterpolator scaler = new SplineInterpolator(known);

            double start = known.First().Key;
            double end = known.Last().Key;

            for (double x = start; x <= end; x += Step)
            {
                double y = scaler.GetValue(x);

                if (y > -1e10 && y < 1e10)
                {
                    resultPoints.Add(new Point(x, y));
                }
            }

            return resultPoints;
        }

        public IEnumerable<Point> LagrandePolynomial()
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = _discreteFunction[0].X; x <= _discreteFunction[_discreteFunction.Count - 1].X; x += Step)
            {
                double sum = 0;

                for (int i = 0; i < _discreteFunction.Count; i++)
                {
                    double multiplication = 1;

                    for (int j = 0; j < _discreteFunction.Count; j++)
                    {
                        if (j != i)
                        {
                            multiplication *= (x - _discreteFunction[j].X) / (_discreteFunction[i].X - _discreteFunction[j].X);
                        }
                    }

                    sum += _discreteFunction[i].Y * multiplication;
                }

                resultPoints.Add(new Point(x, sum));
            }

            return resultPoints;
        }

        public IEnumerable<Point> NewtonPolynomial()
        {
            ICollection<Point> newPoints = new List<Point>();

            double h = _discreteFunction[1].X - _discreteFunction[0].X;

            for (double x = _discreteFunction[0].X; x <= _discreteFunction[_discreteFunction.Count - 1].X; x += Step)
            {
                double px = _discreteFunction[0].Y;
                double q = Math.Round((x - _discreteFunction[0].X) / h, 3);

                for (int i = 1; i < _discreteFunction.Count; i++)
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

        public IEnumerable<Point> LeastSquareMethod(int power)
        {
            if (power <= 0)
            {
                throw new ArgumentOutOfRangeException("Степень метода наименьших квадратов должна быть строго больше нуля!");
            }

            if (_discreteFunction.Count < 3)
            {
                throw new ArgumentOutOfRangeException("Для метода наименьших квадратов необходимо минимум три точки!");
            }

            ICollection<Point> resultPoints = new List<Point>();

            double[] values = new double[_discreteFunction.Count];
            double[] keys = new double[_discreteFunction.Count];

            for (int i = 0; i < _discreteFunction.Count; i++)
            {
                values[i] = _discreteFunction[i].X;
                keys[i] = _discreteFunction[i].Y;
            }

            int size = power + 1;

            double[,] coefficientsOfEquation = new double[size, size];
            coefficientsOfEquation[0, 0] = _discreteFunction.Count;

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

            for (double x = _discreteFunction[0].X; x <= _discreteFunction[_discreteFunction.Count - 1].X; x += Step)
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
                ? _discreteFunction[indexY + 1].Y - _discreteFunction[indexY].Y
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
