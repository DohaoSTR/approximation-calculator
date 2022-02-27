using System;
using System.Collections.Generic;

namespace Approximation
{
    public class ApproximationСontinuous
    {
        private readonly IFunction _function;

        public ApproximationСontinuous(IFunction function)
        {
            _function = function;
        }

        public ICollection<Point> ChebyshevPolynomial(int degree, double step, Interval interval)
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = -1; x <= 1; x += step)
            {
                double t = ((interval.End + interval.Start) / 2d) + ((interval.End - interval.Start) * x / 2d);
                double y = 0;

                for (int i = 0; i < degree - 1; i++)
                {
                    double c = FindC(i, degree, interval);
                    double ti = FindT(x, i);
                    y += c * ti;
                }

                y -= FindC(0, degree, interval) / 2d;

                resultPoints.Add(new Point(t, y));
            }

            return resultPoints;
        }

        private double FindFt(double x, Interval interval)
        {
            double tk = ((interval.Start + interval.End) / 2d) + ((interval.End - interval.Start) * x / 2d);

            return _function.GetResult(tk);
        }

        private double FindT(double x, int i)
        {
            return Math.Cos(i * Math.Acos(x));
        }

        private double FindC(int i, int degree, Interval interval)
        {
            double c = 0;

            for (int k = 0; k < degree - 1; k++)
            {
                c += FindFt(FindXk(k, degree), interval) * FindT(FindXk(k, degree), i);
            }
            c *= 2d / degree;

            return c;
        }

        private double FindXk(double k, double degree)
        {
            return Math.Cos((k + 0.5) * Math.PI / degree);
        }

        public ICollection<Point> FourierSeriesMethod(int degree, double step, Interval interval)
        {
            ICollection<Point> resultPoints = new List<Point>();

            for (double x = interval.Start; x <= interval.End; x += step)
            {
                Integration integration = new Integration(_function);

                double y = 1 / Math.PI * integration.GaussMethod(new Interval(-Math.PI, Math.PI), 3) / 2;

                double sum = 0;
                for (int i = 1; i <= degree; i++)
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
        public ICollection<Point> TaylorSeriesMethod(double ambit, int degree, double step, Interval interval)
        {
            List<Point> resultPoints = new List<Point>();
            IReadOnlyList<Point> points = GetPoints(interval, step);

            IReadOnlyList<Point>[] deriviativePoints = new List<Point>[degree + 1];
            deriviativePoints[0] = points;

            for (int i = 1; i < degree; i++)
            {
                Derivation derivation = new Derivation(deriviativePoints[i - 1]);
                deriviativePoints[i] = (IReadOnlyList<Point>)derivation.NewtonPolynomial();
            }

            for (int i = 0; i < points.Count; i++)
            {
                double y = 0;

                for (int j = 0; j < degree; j++)
                {
                    double yo = 0;

                    foreach (Point p in deriviativePoints[j])
                    {
                        if (p.X == ambit)
                        {
                            yo = p.Y;
                        }
                    }

                    y += yo * Math.Pow(points[i].X - ambit, j) / MathExtension.GetFactorial(j);
                }

                resultPoints.Add(new Point(points[i].X, y));
            }

            return resultPoints;
        }

        private IReadOnlyList<Point> GetPoints(Interval interval, double step)
        {
            List<Point> resultPoints = new List<Point>();

            for (double x = interval.Start; x <= interval.End; x += step)
            {
                resultPoints.Add(new Point(x, _function.GetResult(x)));
            }

            return resultPoints;
        }
    }
}
