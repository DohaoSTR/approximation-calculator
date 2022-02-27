using System;
using System.Collections.Generic;
using System.Linq;

namespace Approximation
{
    public class SplineInterpolator
    {
        private readonly double[] _keys;

        private readonly double[] _values;

        private readonly double[] _h;

        private readonly double[] _a;

        /// <param name="nodes">Коллекция известных точек для интерполяции.
        /// Должна содержать не менее двух элементов.</param>
        /// <exception cref="ArgumentNullException">Если переданная коллекция равна null.</exception>
        /// <exception cref="ArgumentException">Если коллекция содержит меньше двух элементов.</exception>
        public SplineInterpolator(IDictionary<double, double> nodes)
        {
            if (nodes == null)
            {
                throw new ArgumentNullException("nodes");
            }

            if (nodes.Count < 2)
            {
                throw new ArgumentException("Необходимо минимум две точки для интерполяции.");
            }

            _keys = nodes.Keys.ToArray();
            _values = nodes.Values.ToArray();

            _a = new double[nodes.Count];
            _h = new double[nodes.Count];

            for (int i = 1; i < nodes.Count; i++)
            {
                _h[i] = _keys[i] - _keys[i - 1];
            }

            if (nodes.Count > 2)
            {
                double[] sub = new double[nodes.Count - 1];
                double[] diag = new double[nodes.Count - 1];
                double[] sup = new double[nodes.Count - 1];

                for (int i = 1; i <= nodes.Count - 2; i++)
                {
                    diag[i] = (_h[i] + _h[i + 1]) / 3;
                    sup[i] = _h[i + 1] / 6;
                    sub[i] = _h[i] / 6;
                    _a[i] = (_values[i + 1] - _values[i]) / _h[i + 1] - (_values[i] - _values[i - 1]) / _h[i];
                }

                SolveTridiag(sub, diag, sup, ref _a, nodes.Count - 2);
            }
        }

        /// <summary>
        /// Получает интерполированного значение для указанного аргумента.
        /// </summary>
        /// <param name="key">Значение аргумента для интерполяции.</param>
        public double GetValue(double key)
        {
            int gap = 0;
            double previous = double.MinValue;

            for (int i = 0; i < _keys.Length; i++)
            {
                if (_keys[i] < key && _keys[i] > previous)
                {
                    previous = _keys[i];
                    gap = i + 1;
                }
            }

            gap = Math.Max(gap, 1);
            gap = Math.Min(gap, _h.Length - 1);

            double x1 = key - previous;
            double x2 = _h[gap] - x1;

            return ((-_a[gap - 1] / 6 * (x2 + _h[gap]) * x1 + _values[gap - 1]) * x2 +
                (-_a[gap] / 6 * (x1 + _h[gap]) * x2 + _values[gap]) * x1) / _h[gap];
        }

        /// <summary>
        /// Решает линейную систему с трёхдиагональной матрицей размера n*n
        /// </summary>
        private static void SolveTridiag(double[] sub, double[] diag, double[] sup, ref double[] b, int n)
        {
            int index;

            for (index = 2; index <= n; index++)
            {
                sub[index] = sub[index] / diag[index - 1];
                diag[index] = diag[index] - sub[index] * sup[index - 1];
                b[index] = b[index] - sub[index] * b[index - 1];
            }

            b[n] = b[n] / diag[n];

            for (index = n - 1; index >= 1; index--)
            {
                b[index] = (b[index] - sup[index] * b[index + 1]) / diag[index];
            }
        }
    }
}
