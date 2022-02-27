using System;

namespace Approximation
{
    public struct Interval
    {
        public double Start { get; private set; }

        public double End { get; private set; }

        public Interval(double start, double end)
        {
            if (start <= end)
            {
                Start = start;
                End = end;
            }
            else
            {
                throw new ArgumentException("Значение начала интервала должно быть меньше чем значение его конца!");
            }
        }
    }
}
