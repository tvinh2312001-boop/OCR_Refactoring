using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Common.Enums;

namespace DensoMTecGaugeReader.Core.Faces
{
    /// <summary>
    /// Square gauge face domain model.
    /// Represents the geometry of a rectangular dial.
    /// </summary>
    public class SquareFace : IGaugeFace
    {
        public GaugeFaceType FaceType => GaugeFaceType.Square;

        public int Width { get; }
        public int Height { get; }

        public SquareFace(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Normalize a point relative to the square face size.
        /// Returns (x, y) between 0 and 1.
        /// </summary>
        public (double X, double Y) GetNormalizedPosition((int X, int Y) point)
        {
            return ((double)point.X / Width, (double)point.Y / Height);
        }
    }
}