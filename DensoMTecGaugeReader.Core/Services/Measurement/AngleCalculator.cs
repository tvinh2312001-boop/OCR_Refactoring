using OpenCvSharp;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Infrastructure.Utils;

namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Provides angle calculation and normalization logic.
    /// </summary>
    public static class AngleCalculator
    {
        public static double Normalize(GaugeFaceInfo face, GaugeHandInfo hand)
        {
            // baseline: horizontal right
            var baseline = new LineSegmentPoint(face.Center, new Point(face.Center.X + (int)face.Radius, face.Center.Y));
            double angle = GeometryUtils.CalculateAngle(baseline, hand.Line);

            if (hand.Line.P2.Y < face.Center.Y)
                angle = 360 - angle;

            return angle;
        }
    }
}