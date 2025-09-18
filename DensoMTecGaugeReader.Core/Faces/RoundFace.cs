using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Common.Enums;

namespace DensoMTecGaugeReader.Core.Faces
{
    /// <summary>
    /// Round gauge face domain model.
    /// Represents the geometry of a circular dial.
    /// </summary>
    public class RoundFace : IGaugeFace
    {
        public GaugeFaceType FaceType => GaugeFaceType.Round;

        public double Radius { get; }
        public (int X, int Y) Center { get; }

        public RoundFace(double radius, (int X, int Y) center)
        {
            Radius = radius;
            Center = center;
        }

        /// <summary>
        /// Calculate the angle of a point relative to the face center.
        /// </summary>
        public double GetAngleFromPoint((int X, int Y) point)
        {
            var dx = point.X - Center.X;
            var dy = Center.Y - point.Y; // trục Y ngược trong ảnh
            var angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
            return angle < 0 ? angle + 360 : angle;
        }
    }
}