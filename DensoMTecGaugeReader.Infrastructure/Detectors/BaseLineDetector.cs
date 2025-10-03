using System;
using System.Collections.Generic;
using System.Linq;
using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Infrastructure.Models;
using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Detectors
{
    /// <summary>
    /// Estimates the baseline (start-angle) from detected numbers.
    /// Useful to auto-calibrate GaugeConfig.StartAngle.
    /// </summary>
    public class BaseLineDetector
    {
        public double EstimateBaselineAngle(GaugeFaceInfo face, IEnumerable<AnalogGaugeNumber> numbers, GaugeConfig config)
        {
            if (numbers == null) numbers = Enumerable.Empty<AnalogGaugeNumber>();

            // 1) exact MinValue label
            var exact = numbers
                .Select(n => (n, Parsed: ParseOrNull(n.Value)))
                .Where(t => t.Parsed.HasValue && NearlyEqual(t.Parsed.Value, config.MinValue))
                .Select(t => t.n)
                .FirstOrDefault();

            if (exact != null) return AngleOf(face, exact);

            // 2) smallest numeric label
            var smallest = numbers
                .Select(n => (n, Parsed: ParseOrNull(n.Value)))
                .Where(t => t.Parsed.HasValue)
                .OrderBy(t => t.Parsed.Value)
                .Select(t => t.n)
                .FirstOrDefault();

            if (smallest != null) return AngleOf(face, smallest);

            // 3) fallback: straight up (12 o'clock)
            return 90.0;
        }

        public GaugeHandInfo BuildBaselineAsHand(GaugeFaceInfo face, double baselineAngleDeg, double lengthFactor = 0.9)
        {
            double rad = baselineAngleDeg * (Math.PI / 180.0);
            double dx = Math.Cos(rad) * face.Radius * lengthFactor;
            double dy = Math.Sin(rad) * face.Radius * lengthFactor;

            return new GaugeHandInfo
            {
                StartX = face.CenterX,
                StartY = face.CenterY,
                EndX = face.CenterX + dx,
                EndY = face.CenterY + dy,
                Angle = baselineAngleDeg
            };
        }

        private static double AngleOf(GaugeFaceInfo face, AnalogGaugeNumber num)
        {
            // Use GeometryUtils.CalculateAngle by building two line segments:
            // ref line: center -> +X axis; point line: center -> number
            var center = new Point((int)face.CenterX, (int)face.CenterY);
            var refEnd = new Point(center.X + (int)face.Radius, center.Y); // +X direction
            var ptEnd = new Point((int)num.X, (int)num.Y);

            var lineRef = new LineSegmentPoint(center, refEnd);
            var lineNum = new LineSegmentPoint(center, ptEnd);

            double deg = GeometryUtils.CalculateAngle(lineRef, lineNum);
            // Ensure [0, 360)
            if (deg < 0) deg += 360.0;
            if (deg >= 360.0) deg -= 360.0;
            return deg;
        }

        private static double? ParseOrNull(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            if (double.TryParse(s.Replace(',', '.'), System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out var v))
                return v;
            return null;
        }

        private static bool NearlyEqual(double a, double b, double eps = 1e-3) => Math.Abs(a - b) <= eps;
    }
}
