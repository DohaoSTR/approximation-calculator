using System;

namespace StandardMath
{
    public struct Interval
    {
        public double Start { get; private set; }

        public double End { get; private set; }

        public Interval(double start, double end)
        {
            if (start >= end)
            {
                throw new ArgumentException("Значение начала интервала должно быть меньше чем значение его конца!");
            }

            Start = start;
            End = end;
        }

        public bool IsIntervalContain(double value)
        {
            return value >= Start && value <= End;
        }
    }
}