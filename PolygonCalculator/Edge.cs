namespace PolygonCalculator
{
    using System;

    /// <summary>
    ///  Provides the ability to create the edge as element of the polygon.
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class using polygon and index of point.
        /// </summary>
        /// <param name="polygon">An instance of the <see cref="Polygon"/> class.</param>
        /// <param name="index">An index of the second point of the edge.</param>
        public Edge(Polygon polygon, int index)
        {
            this.CurrPoint = polygon.Points[index];
            if (index != 0)
            {
                this.PrevPoint = polygon.Points[index - 1];
            }
            else
            {
                this.PrevPoint = polygon.Points[polygon.Points.Length - 1];
            }

            if (CurrPoint.Equals(PrevPoint))
            {
                throw new ArgumentException("This edge has points with the same coordinates", nameof(polygon));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Edge"/> class by two points.
        /// </summary>
        /// <param name="prev">An instance of the <see cref="Point"/> class.</param>
        /// <param name="cur">An instance of the <see cref="Point"/> class.</param>
        public Edge(Point prev, Point cur)
        {
            this.PrevPoint = prev;
            this.CurrPoint = cur;
        }

        /// <summary>
        /// Gets second point of the edge.
        /// </summary>
        public Point CurrPoint { get; }

        /// <summary>
        /// Gets first point of the edge.
        /// </summary>
        public Point PrevPoint { get; }

        /// <summary>
        /// Calculates point of intersection if it exists.
        /// </summary>
        /// <param name="e1">An instance of the <see cref="Edge"/> class.</param>
        /// <param name="e2">An instance of the <see cref="Edge"/> class.</param>
        /// <param name="crossPoint">An instance of the <see cref="Point"/> class.</param>
        /// <returns>A bool value of the crossing point existence.</returns>
        public static bool TryFindIntersection(Edge e1, Edge e2, out Point crossPoint)
        {
            float a1 = e1.CurrPoint.Y - e1.PrevPoint.Y;
            float b1 = e1.PrevPoint.X - e1.CurrPoint.X;
            float c1 = a1 * e1.PrevPoint.X + b1 * e1.PrevPoint.Y;

            float a2 = e2.CurrPoint.Y - e2.PrevPoint.Y;
            float b2 = e2.PrevPoint.X - e2.CurrPoint.X;
            float c2 = a2 * e2.PrevPoint.X + b2 * e2.PrevPoint.Y;

            float delta = a1 * b2 - a2 * b1;

            if (delta == 0)
            {
                crossPoint = new Point(float.NaN, float.NaN);
                return false;
            }
            float x = (b2 * c1 - b1 * c2) / delta;
            float y = (a1 * c2 - a2 * c1) / delta;

            if (e1.IsOutside(x, y) || e2.IsOutside(x, y))
            {
                crossPoint = new Point(float.NaN, float.NaN);
                return false;
            }

            crossPoint = new Point(Convert.ToSingle(Math.Round(x, 4)), Convert.ToSingle(Math.Round(y, 4)));
            return true;
        }

        private bool IsOutside(float x, float y)
        {
            return (x < PrevPoint.X && x < CurrPoint.X)
                || (x > PrevPoint.X && x > CurrPoint.X)
                || (y < PrevPoint.Y && y < CurrPoint.Y)
                || (y > PrevPoint.Y && y > CurrPoint.Y);
        }
    }
}
