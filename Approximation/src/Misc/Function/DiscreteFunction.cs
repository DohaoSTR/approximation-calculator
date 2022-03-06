using System;
using System.Collections;
using System.Collections.Generic;

namespace Approximation
{
    public class DiscreteFunction : IEnumerable<Point>
    {
        private readonly List<Point> _points;

        public int Count => _points.Count;

        public DiscreteFunction(IEnumerable<Point> points)
        {
            if (points == null)
            {
                throw new NullReferenceException("Список points не должен быть равен null!");
            }

            if (CheckEqualPoints((IReadOnlyList<Point>)points))
            {
                throw new ArgumentOutOfRangeException("В дискретной функции не должно быть повторяющихся точек!");
            }

            if (CheckEqualValues((IReadOnlyList<Point>)points))
            {
                throw new ArgumentOutOfRangeException("В дискретной функции не должно быть точек с одинаковым значением (x)!");
            }

            _points = (List<Point>)points;
        }

        public DiscreteFunction()
        {
            _points = new List<Point>();
        }

        public Point this[int index]
        {
            get
            {
                return _points[index];
            }
        }

        public IReadOnlyCollection<Point> GetPoints() => _points;

        public int Add(Point addPoint)
        {
            if (CheckEqualPoint(addPoint, _points))
            {
                throw new ArgumentOutOfRangeException("Добавляемая точка не должна быть равна уже добавленной точке!");
            }

            if (CheckEqualValue(addPoint, _points))
            {
                throw new ArgumentOutOfRangeException("Значение добавляемой точки (x) - не должно быть равно уже существующей точке!");
            }

            if (addPoint == null)
            {
                throw new NullReferenceException("Добавляемая точка не должна быть равна null!");
            }

            _points.Add(addPoint);

            return _points.FindIndex(point => point == addPoint);
        }

        public bool Remove(Point removePoint)
        {
            return _points.Remove(removePoint);
        }

        public void RemoveAt(int removeIndex)
        {
            _points.RemoveAt(removeIndex);
        }

        public void SortValueMax()
        {
            for (int index = 0; index < _points.Count; index++)
            {
                Point currentPoint = _points[index];

                for (int nextIndex = index; nextIndex < _points.Count; nextIndex++)
                {
                    Point nextPoint = _points[nextIndex];

                    if (currentPoint.X > nextPoint.X)
                    {
                        Point temporaryPoint = nextPoint;
                        _points[nextIndex] = currentPoint;
                        _points[index] = temporaryPoint;
                    }
                }
            }
        }

        private bool CheckEqualPoint(Point checkPoint, IEnumerable<Point> points)
        {
            foreach (Point point in points)
            {
                if (checkPoint == point)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEqualValue(Point checkPoint, IEnumerable<Point> points)
        {
            foreach (Point point in points)
            {
                if (checkPoint.X == point.X)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEqualPoints(IReadOnlyList<Point> points)
        {
            for (int index = 0; index < points.Count; index++)
            {
                Point currentPoint = points[index];

                if (CheckEqualPoint(currentPoint, points))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckEqualValues(IReadOnlyList<Point> points)
        {
            for (int index = 0; index < points.Count; index++)
            {
                Point currentPoint = points[index];

                if (CheckEqualValue(currentPoint, points))
                {
                    return true;
                }
            }

            return false;
        }

        public IEnumerator<Point> GetEnumerator()
        {
            foreach (Point point in _points)
            {
                yield return point;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
