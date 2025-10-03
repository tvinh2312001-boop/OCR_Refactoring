using System;
using System.Collections.Generic;
using System.Linq;
using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Contracts.Gauges;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Services.Measurement;
using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Detectors
{
    /// <summary>
    /// Detects the gauge hand/needle via masked ROI + HoughLinesP.
    /// </summary>
    public class GaugeHandDetector : IGaugeHandDetector
    {
        public GaugeHandInfo DetectHand(string imagePath, GaugeFaceInfo faceInfo)
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (src.Empty())
                throw new ArgumentException("Image not found or cannot be read.", nameof(imagePath));

            // circular mask (fix MatExpr -> Mat)
            using var mask = new Mat(src.Size(), MatType.CV_8UC1, Scalar.Black);
            Cv2.Circle(
                mask,
                new Point((int)faceInfo.CenterX, (int)faceInfo.CenterY),
                (int)(faceInfo.Radius * 0.98),
                Scalar.White,
                thickness: -1);

            using var roi = new Mat();
            Cv2.BitwiseAnd(src, src, roi, mask);

            using var gray = new Mat();
            using var blur = new Mat();
            using var edges = new Mat();

            Cv2.CvtColor(roi, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, blur, new Size(5, 5), 1.5);
            Cv2.Canny(blur, edges, 60, 150);

            int minLen = (int)(faceInfo.Radius * 0.35);
            int maxGap = (int)(faceInfo.Radius * 0.05);

            var lines = Cv2.HoughLinesP(
                edges,
                rho: 1,
                theta: Math.PI / 180,
                threshold: 40,
                minLineLength: Math.Max(15, minLen),
                maxLineGap: Math.Max(5, maxGap));

            if (lines == null || lines.Length == 0)
                throw new InvalidOperationException("No needle-like line detected.");

            var center = new Point((int)faceInfo.CenterX, (int)faceInfo.CenterY);
            var circle = new CircleSegment(new Point2f((float)faceInfo.CenterX, (float)faceInfo.CenterY), (float)faceInfo.Radius);

            var candidates = new List<(LineSegmentPoint Line, double Score, OpenCvSharp.Point FarEnd)>();

            foreach (var l in lines)
            {
                // line validity: inside face (allow small slack)
                if (!GeometryUtils.IsLineInsideCircle(l, circle, thresholdRatio: 1.02))
                    continue;

                var p1 = l.P1;
                var p2 = l.P2;
                double d1 = GeometryUtils.GetDistance(center, p1);
                double d2 = GeometryUtils.GetDistance(center, p2);

                var near = d1 < d2 ? p1 : p2;
                var far = d1 < d2 ? p2 : p1;

                // near endpoint must be close to center
                if (GeometryUtils.GetDistance(center, near) > faceInfo.Radius * 0.35) continue;

                // far endpoint should be long enough and not past the rim
                double dFar = GeometryUtils.GetDistance(center, far);
                if (dFar < faceInfo.Radius * 0.45) continue;
                if (dFar > faceInfo.Radius * 1.05) continue;

                double len = GeometryUtils.GetDistance(p1, p2);
                double score = len;
                candidates.Add((l, score, far));
            }

            if (candidates.Count == 0)
                throw new InvalidOperationException("No valid needle candidate found.");

            var best = candidates.OrderByDescending(c => c.Score).First();

            var hand = new GaugeHandInfo
            {
                StartX = faceInfo.CenterX,
                StartY = faceInfo.CenterY,
                EndX = best.FarEnd.X,
                EndY = best.FarEnd.Y
            };

            var angleCalc = new AngleCalculator();
            hand.Angle = angleCalc.CalculateAngle(faceInfo, hand);

            var start = new Point((int)hand.StartX, (int)hand.StartY);
            var end = new Point((int)hand.EndX, (int)hand.EndY);

            Cv2.Line(src, start, end, Scalar.Red, 2);

            Cv2.Circle(src, start, 5, Scalar.Blue, -1);

            Cv2.ImShow("Gauge Hand", src);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
            return hand;
        }
    }
}
