namespace PolygonCalculator
{
    /// <summary>
    /// Provides the ability to create the point with coordinates on a plane.
    /// </summary>
    public struct Point
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">An float precision number of horizontal coordinate value.</param>
        /// <param name="y">An float precision number of vertical coordinate value.</param>
        public Point(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets horizontal coordinate value.
        /// </summary>
        public float X { get; }

        /// <summary>
        /// Gets vertical coordinate value.
        /// </summary>
        public float Y { get; }
    }
}
