namespace PolygonCalculator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides the ability to create polygon by points and basic geometrical operations.
    /// </summary>
    public class Polygon
    {
        /// <summary>
        /// Maximum available count of points to creating the polygon.
        /// </summary>
        private const int limit = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="Polygon"/> class.
        /// </summary>
        /// <param name="points">An array of polygon points.</param>
        public Polygon(params Point[] points)
        {
            if (points.Length < 3)
            {
                throw new ArgumentException("This points aren't enough to creating a polygon", nameof(points));
            }

            this.Points = points;
        }

        /// <summary>
        /// Gets value of point array.
        /// </summary>
        public Point[] Points { get; }

        /// <summary>
        /// Gets or sets value of point index.
        /// </summary>
        private int CurrentPointIndex { get; set; }

        /// <summary>
        /// Increments param number or resets to zero if it equals the last index of array.
        /// </summary>
        /// <param name="index">A current index of the Point</param>
        /// <returns>An integer value of the next index</returns>
        private int GetNextIndex(int index)
        {
            return index < this.Points.Length - 1 ? ++index : 0;
        }

        /// <summary>
        /// Calculates intersection of two polygons.
        /// </summary>
        /// <param name="polygonA">An instance of the <see cref="Polygon"/> class.</param>
        /// <param name="polygonB">An instance of the <see cref="Polygon"/> class.</param>
        /// <returns>A new instance of the <see cref="Polygon"/> class.</returns>
        public static Polygon Intersect(Polygon polygonA, Polygon polygonB)
        {
            if (TrySearchOverallPoint(polygonA, polygonB, out Point firstCrossPoint))
            {
                return CalculateIntersection(polygonA, polygonB, firstCrossPoint);
            }
            else
            {
                if (polygonA.Surround(polygonB.Points[0]))
                {
                    return polygonB;
                }
                else if (polygonB.Surround(polygonA.Points[0]))
                {
                    return polygonA;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Calculates merger of two polygons.
        /// </summary>
        /// <param name="polygonA">An instance of the <see cref="Polygon"/> class.</param>
        /// <param name="polygonB">An instance of the <see cref="Polygon"/> class.</param>
        /// <returns>A new instance of the <see cref="Polygon"/> class.</returns>
        public static Polygon Merge(Polygon polygonA, Polygon polygonB)
        {
            if (TrySearchOverallPoint(polygonA, polygonB, out Point firstCrossPoint))
            {
                return CalculateMerger(polygonA, polygonB, firstCrossPoint);
            }
            else
            {
                if (polygonA.Surround(polygonB.Points[0]))
                {
                    return polygonA;
                }
                else if (polygonB.Surround(polygonA.Points[0]))
                {
                    return polygonB;
                }
                else
                {
                    return new Polygon(polygonA.Points.Concat(polygonB.Points).ToArray());
                }
            }
        }

        /// <summary>
        /// Calculates the area of the polygon.
        /// </summary>
        /// <returns>A double value of an area.</returns>
        public double GetArea()
        {
            double sum = 0;
            for (int i = 0; i < this.Points.Length; i++)
            {
                sum += (this.Points[i].X * this.Points[GetNextIndex(i)].Y) - (this.Points[GetNextIndex(i)].X * this.Points[i].Y);
            }

            return Math.Abs(sum / 2);
        }

        /// <summary>
        /// Calculates location of the point, inside or outside the polygon.
        /// </summary>
        /// <param name="point">An instance of the <see cref="Point"/> class.</param>
        /// <returns>A bool value of the point location.</returns>
        private bool Surround(Point point)
        {
            int i1, i2;
            float s, s1, s2, s3;
            bool flag = false;
            for (int i = 0; i < this.Points.Length; i++)
            {
                flag = false;
                i1 = GetNextIndex(i);
                while (!flag)
                {
                    i2 = GetNextIndex(i1);
                    if (i2 == GetNextIndex(i))
                    {
                        break;
                    }

                    s = Math.Abs((this.Points[i1].X * (this.Points[i2].Y - this.Points[i].Y)) +
                    (this.Points[i2].X * (this.Points[i].Y - this.Points[i1].Y)) +
                    (this.Points[i].X * (this.Points[i1].Y - this.Points[i2].Y)));

                    s1 = Math.Abs((this.Points[i1].X * (this.Points[i2].Y - point.Y)) +
                    (this.Points[i2].X * (point.Y - this.Points[i1].Y)) +
                    (point.X * (this.Points[i1].Y - this.Points[i2].Y)));

                    s2 = Math.Abs((this.Points[i].X * (this.Points[i2].Y - point.Y)) +
                    (this.Points[i2].X * (point.Y - this.Points[i].Y)) +
                    (point.X * (this.Points[i].Y - this.Points[i2].Y)));

                    s3 = Math.Abs((this.Points[i1].X * (this.Points[i].Y - point.Y)) +
                    (this.Points[i].X * (point.Y - this.Points[i1].Y)) +
                    (point.X * (this.Points[i1].Y - this.Points[i].Y)));

                    if (s == s1 + s2 + s3)
                    {
                        flag = true;
                        break;
                    }

                    i1 = GetNextIndex(i1);
                }

                if (!flag)
                {
                    break;
                }
            }

            return flag;
        }

        /// <summary>
        /// Calculates overall point of polygons if it exists.
        /// </summary>
        /// <param name="polygonA">An instance of the <see cref="Polygon"/> class.</param>
        /// <param name="polygonB">An instance of the <see cref="Polygon"/> class.</param>
        /// <param name="firstCrossPoint">An instance of the <see cref="Point"/> class.</param>
        /// <returns>A bool value of the overall point existence.</returns>
        private static bool TrySearchOverallPoint(Polygon polygonA, Polygon polygonB, out Point firstCrossPoint)
        {
            for (int i = 0; i < polygonA.Points.Length; i++)
            {
                for (int j = 0; j < polygonB.Points.Length; j++)
                {
                    if (Edge.TryFindIntersection(new Edge(polygonA, i), new Edge(polygonB, j), out firstCrossPoint))
                    {
                        polygonA.CurrentPointIndex = i;
                        polygonB.CurrentPointIndex = j;
                        return true;
                    }
                }
            }
            firstCrossPoint = new Point(0f, 0f);
            return false;
        }

        private static Polygon CalculateIntersection(Polygon polygonA, Polygon polygonB, Point firstCrossPoint)
        {
            Point? crossPoint = null;
            IList<Point> result = new List<Point>
            {
                firstCrossPoint
            };
            bool switcher = polygonA.Surround(polygonB.Points[polygonB.CurrentPointIndex]) ||
                (!Edge.TryFindIntersection(new Edge(polygonA, polygonA.CurrentPointIndex), new Edge(polygonB, polygonB.GetNextIndex(polygonB.CurrentPointIndex)), out Point point)
                && !polygonB.Surround(polygonA.Points[polygonA.CurrentPointIndex]));
            do
            {
                if (crossPoint.HasValue)
                {
                    result.Add(crossPoint.Value);
                    if (result.Count > limit)
                    {
                        throw new ApplicationException("Miscalculation in the construction of the polygon");
                    }
                }
                if (switcher)
                {
                    crossPoint = DefineIntersectPoints(polygonB, polygonA, result);
                }
                else
                {
                    crossPoint = DefineIntersectPoints(polygonA, polygonB, result);
                }
                switcher = !switcher;
            }
            while (!crossPoint.Value.Equals(firstCrossPoint));
            return new Polygon(result.ToArray());
        }

        private static Point DefineIntersectPoints(Polygon innerPolygon, Polygon outerPolygon, IList<Point> result)
        {
            Point crossPoint;
            while (outerPolygon.Surround(innerPolygon.Points[innerPolygon.CurrentPointIndex]))
            {
                result.Add(innerPolygon.Points[innerPolygon.CurrentPointIndex]);
                innerPolygon.CurrentPointIndex = innerPolygon.GetNextIndex(innerPolygon.CurrentPointIndex);
            }
            outerPolygon.CurrentPointIndex = outerPolygon.GetNextIndex(outerPolygon.CurrentPointIndex);
            while (!Edge.TryFindIntersection(new Edge(innerPolygon, innerPolygon.CurrentPointIndex), new Edge(outerPolygon, outerPolygon.CurrentPointIndex), out crossPoint))
            {
                outerPolygon.CurrentPointIndex = outerPolygon.GetNextIndex(outerPolygon.CurrentPointIndex);
            }

            return crossPoint;
        }

        private static Polygon CalculateMerger(Polygon polygonA, Polygon polygonB, Point firstCrossPoint)
        {
            bool afterCross = true;
            IList<Point> result = new List<Point>
            {
                firstCrossPoint
            };

            bool switcher = polygonA.Surround(polygonB.Points[polygonB.CurrentPointIndex]);

            while (true)
            {
                if (afterCross && Edge.TryFindIntersection(new Edge(polygonA, polygonA.GetNextIndex(polygonA.CurrentPointIndex)), new Edge(polygonB, polygonB.CurrentPointIndex), out Point point))
                {
                    afterCross = false;
                }              
                if (DefineMergerPoints(switcher ? polygonA : polygonB, switcher ? polygonB : polygonA, result, ref afterCross)
                            || DefineMergerPoints(switcher ? polygonB : polygonA, switcher ? polygonA : polygonB, result, ref afterCross))
                {
                    return new Polygon(result.ToArray());
                }           
            }
        }

        private static bool DefineMergerPoints(Polygon innerPolygon, Polygon outerPolygon, IList<Point> result, ref bool afterCross)
        {
            Point crossPoint;
            while (outerPolygon.Surround(innerPolygon.Points[innerPolygon.CurrentPointIndex]))
            {
                innerPolygon.CurrentPointIndex = innerPolygon.GetNextIndex(innerPolygon.CurrentPointIndex);
            }

            if (!afterCross)
            {
                while (true)
                {
                    if (!innerPolygon.Surround(outerPolygon.Points[outerPolygon.CurrentPointIndex]))
                    {
                        result.Add(outerPolygon.Points[outerPolygon.CurrentPointIndex]);
                        afterCross = true;
                    }
                    outerPolygon.CurrentPointIndex = outerPolygon.GetNextIndex(outerPolygon.CurrentPointIndex);

                    if (Edge.TryFindIntersection(new Edge(outerPolygon, outerPolygon.CurrentPointIndex), new Edge(innerPolygon, innerPolygon.CurrentPointIndex), out crossPoint))
                    {
                        if (crossPoint.Equals(result.First()))
                        {
                            return true;
                        }

                        result.Add(crossPoint);
                        if (result.Count > limit)
                        {
                            throw new ApplicationException("Miscalculation in the construction of the polygon");
                        }
                        if (afterCross && !outerPolygon.Surround(innerPolygon.Points[innerPolygon.CurrentPointIndex]))
                        {
                            break;
                        }
                    }
                }
            }

            result.Add(innerPolygon.Points[innerPolygon.CurrentPointIndex]);
            afterCross = false;
            innerPolygon.CurrentPointIndex = innerPolygon.GetNextIndex(innerPolygon.CurrentPointIndex);
            if (Edge.TryFindIntersection(new Edge(outerPolygon, outerPolygon.CurrentPointIndex), new Edge(innerPolygon, innerPolygon.CurrentPointIndex), out crossPoint))
            {
                if (crossPoint.Equals(result.First()))
                {
                    return true;
                }

                result.Add(crossPoint);
                if (result.Count > limit)
                {
                    throw new ApplicationException("Miscalculation in the construction of the polygon");
                }
                afterCross = true;
            }

            return false;
        }
    }
}
