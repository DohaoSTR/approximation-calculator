using System;
using System.Collections.Generic;

namespace Approximation
{
    public class ApproximationСontinuous
    {
        private readonly IFunction _function;

        public string Function => _function.ToString();

        public Interval Interval { get; private set; }

        public int Power { get; private set; }

        public double Step { get; private set; }

        public ApproximationСontinuous(IFunction function, Interval interval, int power, double step)
        {
            if (power <= 0)
            {
                throw new ArgumentOutOfRangeException("Степень методов должна быть строго больше нуля!");
            }

            if (step <= 0)
            {
                throw new ArgumentOutOfRangeException("Шаг должен быть больше нуля!");
            }

            _function = function;
            Interval = interval;
            Power = power;
            Step = step;
        }

        public IEnumerable<Point> ChebyshevPolynomial()
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = -1; x <= 1; x += Step)
            {
                double t = ((Interval.End + Interval.Start) / 2d) + ((Interval.End - Interval.Start) * x / 2d);
                double y = 0;

                for (int i = 0; i < Power - 1; i++)
                {
                    double c = FindC(i, Power);
                    double ti = FindT(x, i);
                    y += c * ti;
                }

                y -= FindC(0, Power) / 2d;

                resultPoints.Add(new Point(t, y));
            }

            return resultPoints;
        }

        private double FindFt(double x)
        {
            double tk = ((Interval.Start + Interval.End) / 2d) + ((Interval.End - Interval.Start) * x / 2d);

            return _function.GetResult(tk);
        }

        private double FindT(double x, int i)
        {
            return Math.Cos(i * Math.Acos(x));
        }

        private double FindC(int i, int degree)
        {
            double c = 0;

            for (int k = 0; k < degree - 1; k++)
            {
                c += FindFt(FindXk(k, degree)) * FindT(FindXk(k, degree), i);
            }
            c *= 2d / degree;

            return c;
        }

        private double FindXk(double k, double degree)
        {
            return Math.Cos((k + 0.5) * Math.PI / degree);
        }

        public IEnumerable<Point> FourierSeriesMethod()
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = Interval.Start; x <= Interval.End; x += Step)
            {
                Integration integration = new Integration(_function);

                double y = 1 / Math.PI * integration.GaussMethod(new Interval(-Math.PI, Math.PI), 3) / 2;

                double sum = 0;
                for (int i = 1; i <= Power; i++)
                {
                    sum += (FindA(i) * Math.Cos(i * x)) + (FindB(i) * Math.Sin(i * x));
                }
                y += sum;

                resultPoints.Add(new Point(x, y));
            }
            return resultPoints;
        }

        private double FindA(int n)
        {
            Function function = new Function(_function.ToString() + "*cos(x*" + n + ")");
            Integration integration = new Integration(function);

            return 1 / Math.PI * integration.GaussMethod(new Interval(-Math.PI, Math.PI), 3);
        }

        private double FindB(int n)
        {
            Function function = new Function(_function.ToString() + "*sin(x*" + n + ")");
            Integration integration = new Integration(function);

            double integral = integration.ParabolaMethod(new Interval(-Math.PI, Math.PI), 0.001);

            return integral / Math.PI;
        }

        /// <param name="ambit">Окрестность.</param>
        public IEnumerable<Point> TaylorSeriesMethod(double ambit)
        {
            if (Interval.IsIntervalContain(ambit) == false)
            {
                throw new ArgumentOutOfRangeException("Окрестность должна входить в интервал!");
            }

            List<Point> resultPoints = new List<Point>();
            IReadOnlyList<Point> points = (IReadOnlyList<Point>)GetPoints();

            IReadOnlyList<Point>[] deriviativePoints = new List<Point>[Power];
            deriviativePoints[0] = points;

            for (int i = 1; i < Power; i++)
            {
                Derivation derivation = new Derivation(deriviativePoints[i - 1]);
                deriviativePoints[i] = (IReadOnlyList<Point>)derivation.NewtonPolynomial();
            }

            for (int i = 0; i < points.Count; i++)
            {
                double y = 0;

                for (int j = 0; j < Power; j++)
                {
                    double yo = 0;

                    foreach (Point point in deriviativePoints[j])
                    {
                        if (point.X == ambit)
                        {
                            yo = point.Y;
                        }
                    }

                    if (j == 0)
                    {
                        y += yo;
                    }
                    else
                    {
                        y += yo * Math.Pow(points[i].X - ambit, j) / MathExtension.GetFactorial(j);
                    }
                }

                resultPoints.Add(new Point(points[i].X, y));
            }

            return resultPoints;
        }

        private IEnumerable<Point> GetPoints()
        {
            List<Point> resultPoints = new List<Point>();

            for (double x = Interval.Start; x <= Interval.End; x += Step)
            {
                resultPoints.Add(new Point(x, _function.GetResult(x)));
            }

            return resultPoints;
        }
    }
}
