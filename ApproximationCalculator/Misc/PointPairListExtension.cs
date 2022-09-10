using StandardMath;
using System.Collections.Generic;
using ZedGraph;

namespace ApproximationCalculator
{
    public static class PointPairListExtension
    {
        public static PointPairList ConvertToPointPairList(this PointPairList pointPairList, IEnumerable<Point> points)
        {
            PointPairList resultPoints = new PointPairList();

            foreach (Point point in points)
            {
                resultPoints.Add(new PointPair(point.X, point.Y));
            }

            return resultPoints;
        }

        public static IEnumerable<Point> ConvertToIEnumerable(this PointPairList points)
        {
            List<Point> resultPoints = new List<Point>();

            foreach (PointPair point in points)
            {
                resultPoints.Add(new Point(point.X, point.Y));
            }

            return resultPoints;
        }
    }
}