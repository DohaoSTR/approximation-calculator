using System;
using System.Collections.Generic;

namespace Approximation
{
    public class ApproximationError
    {
        private readonly List<Point> _points;

        public ApproximationError(IEnumerable<Point> points)
        {
            _points = (List<Point>)points;
        }

        /// <returns>Возвращает cреднеквадратичное отклонение. 
        /// В общем смысле среднеквадратическое отклонение можно считать мерой неопределённости.
        /// В случае аппроксимации это среднее значение отклонения от искомой функции на определенном отрезке.
        /// </returns>
        public double RootSquareDeviation()
        {
            double sumValues = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                sumValues += _points[i].X;
            }

            sumValues /= _points.Count;

            double sumDeviation = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                sumDeviation += Math.Abs(Math.Pow(_points[i].Y - sumValues, 2));
            }

            return Math.Sqrt(sumDeviation / _points.Count);
        }

        /// <returns>Возвращает абсолютное отклонение.</returns>
        /// <remarks>Возвращает абсолютную разница между элементом и выбранной точкой, 
        /// от которой отсчитывается отклонение.</remarks>
        public double AbsoluteDeviation()
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

        /// <returns>Возвращает абсолютное отклонение для каждой точки.</returns>
        public List<double> AbsoluteDeviationList()
        {
            List<double> deviations = new List<double>();

            double sum = 0;
            for (int i = 0; i < _points.Count; i++)
            {
                sum += _points[i].X;
            }

            sum /= _points.Count;

            for (int i = 0; i < _points.Count; i++)
            {
                double s = Math.Abs(_points[i].Y - sum);

                deviations.Add(s * 100);
            }

            return deviations;
        }

        /// <remarks>В отличии от просто абсолютного отклонения в 
        /// качестве среднего значения берётся медиана.</remarks>
        /// <returns>Значение среднего абсолютного отклонения</returns>
        public double MeanAbsoluteDeviation()
        {
            double result = 1 / Convert.ToDouble(_points.Count);
            double sum = 0;
            double medianValue = GetMedianValue();

            for (int i = 0; i < _points.Count; i++)
            {
                sum += Math.Abs(_points[i].Y - medianValue);
            }

            result *= sum;

            return result;
        }

        private double GetMedianValue()
        {
            double indexMedianValue = _points.Count / 2;

            double medianValue;

            if (_points.Count % 2 == 0)
            {
                medianValue = _points[Convert.ToInt32(indexMedianValue)].Y;
            }
            else
            {
                indexMedianValue -= 0.5;

                medianValue = _points[Convert.ToInt32(indexMedianValue)].Y;
            }

            return medianValue;
        }

        /// <summary>Метод минимума хи-квадрат.</summary>
        /// <returns>Возвращает минимум хи-квадрата. 
        /// Чем ближе значение к нулю, тем лучше построена модель и в ней меньше погрешностей.
        /// </returns>
        public double XSquare(Function function)
        {
            List<double> deviations = AbsoluteDeviationList();
            double sumValueSquare = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                sumValueSquare += Math.Pow((_points[i].Y - function.GetResult(_points[i].X)) / deviations[i], 2);
            }

            return Math.Sqrt(sumValueSquare);
        }

        /// <summary>Метод максимального правдоподобия.</summary>
        /// <returns>Возвращает максимум правдоподобия который должен соответствовать минимуму хи-квадрата. 
        /// Чем ближе значение к нулю, тем лучше построена модель и в ней меньше погрешностей.
        /// </returns>
        public double MaxLikelihood(Function function)
        {
            List<double> deviations = AbsoluteDeviationList();

            double logarithmLikelihood = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                logarithmLikelihood += Math.Pow(_points[i].Y - function.GetResult(_points[i].X), 2) / (2 * Math.Pow(deviations[i], 2));
            }

            return Math.Sqrt(logarithmLikelihood / 2);
        }

        /// <returns>Возвращает относительную ошибку аппроксимации (погрешность) в точке. 
        /// Значение является процентом. 5-7% свидетельствует о хорошем подборе функции к исходным данным.
        /// </returns>
        public double RelativeApproximationError(Function function, Point point)
        {
            double errorValue = Math.Abs((point.Y - function.GetResult(point.X)) / point.Y);

            return errorValue;
        }

        public List<double> RelativeApproximationErrors(Function function)
        {
            List<double> errorsList = new List<double>();

            for (int i = 0; i < _points.Count; i++)
            {
                double errorValue = RelativeApproximationError(function, _points[i]) / _points.Count;

                errorsList.Add(errorValue * 100);
            }

            return errorsList;
        }

        public double AverageRelativeApproximationError(Function function)
        {
            double averageErrorValue = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                averageErrorValue += RelativeApproximationError(function, _points[i]);
            }

            return averageErrorValue * 100 / _points.Count;
        }

        /// <returns>Возвращает оценку модели. 
        /// Чем ближе значение к нулю, тем лучше построена модель и в ней меньше погрешностей.
        /// </returns>
        public double RateLeastSquareMethod(Function function)
        {
            double sum = 0;

            for (int i = 0; i < _points.Count; i++)
            {
                sum += Math.Pow(_points[i].Y - function.GetResult(_points[i].X), 2);
            }

            return sum / _points.Count;
        }

        /// <summary>Метод проверки качества аппроксимации.</summary>
        /// <returns>При хорошем соответствии модели и данных, значение должно в среднем быть равно единице. Значения 
        /// существенно большие (2 и выше) свидетельствуют либо о плохом соответствии теории и результатов измерений, 
        /// либо о заниженных погрешностях. Значения меньше 0,5 как правило свидетельствуют о завышенных погрешностях.
        /// </returns>
        public double ApproximationQuality(Function function)
        {
            return XSquare(function) / (_points.Count - function.GetNumberCoefficients());
        }
    }
}
