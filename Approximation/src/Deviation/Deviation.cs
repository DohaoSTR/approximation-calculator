using System;
using System.Collections.Generic;

namespace Approximation
{
    public class Deviation
    {
        private readonly List<Point> _points;

        public Deviation(IEnumerable<Point> points)
        {
            _points = (List<Point>)points;
        }

        public double RootSquareMethod()
        {
            double sum = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                sum += _points[i].X;
            }

            sum /= _points.Count;

            double s = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                s += Math.Abs(Math.Pow(_points[i].Y - sum, 2));
            }

            return Math.Sqrt(s / _points.Count);
        }

        public double AbsMethod()
        {
            double sum = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                sum += _points[i].X;
            }

            sum /= _points.Count;

            double max = Math.Abs(_points[0].Y - sum);

            for (int i = 0; i < _points.Count; i++)
            {
                double s = Math.Abs(_points[i].Y - sum);

                if (max < s)
                {
                    max = s;
                }
            }

            return max;
        }
    }
}
