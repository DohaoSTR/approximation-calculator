using Approximation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace AnalysisEffectOfError
{
    public static class PointPairListExtension
    {
        public static PointPairList ConvertToPointPairList(this PointPairList list, IEnumerable<Point> points)
        {
            PointPairList resultPoints = new PointPairList();

            foreach (Point point in points)
            {
                resultPoints.Add(new PointPair(point.X, point.Y));
            }

            return resultPoints;
        }

        public static IReadOnlyList<Point> ConvertToReadOnlyList(this PointPairList points)
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
