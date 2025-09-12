using OpenCvSharp;
using DensoMTecGaugeReader.Infrastructure.Utils;

namespace DensoMTecGaugeReader.Core.Services.Measurement
{
    /// <summary>
    /// Provides utilities for angle normalization and calculation.
    /// </summary>
    public static class AngleCalculator
    {
        /// <summary>
        /// Normalize angle based on baseline and hand position.
        /// </summary>
        public static double Normalize(LineSegmentPoint baseLine, LineSegmentPoint handLine)
        {
            double angle = GeometryUtils.CalculateAngle(baseLine, handLine);

            // Adjust for orientation if needed
            if (handLine.P2.Y < baseLine.P2.Y)
                angle = 360 - angle;

            return angle;
        }
    }
}