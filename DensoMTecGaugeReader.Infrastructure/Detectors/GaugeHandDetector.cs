using OpenCvSharp;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Infrastructure.Utils;

namespace DensoMTecGaugeReader.Infrastructure.ImageProcessing
{
    /// <summary>
    /// Detects the hand (needle) of a gauge using line detection (HoughLinesP).
    /// </summary>
    public static class GaugeHandDetector
    {
        public static LineSegmentPoint DetectGaugeHand(Mat image, CircleSegment face)
        {
            Mat preprocessed = GetPreprocessedImageForHandDetection(image, face);

            var detectedLines = Cv2.HoughLinesP(
                preprocessed,
                rho: 1,
                theta: Math.PI / 180,
                threshold: 50,
                minLineLength: 60,
                maxLineGap: 10
            );

            List<Point> handTips = new();
            foreach (var line in detectedLines)
            {
                Point2f direction = line.P2 - line.P1;
                float norm = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
                if (norm == 0) continue;

                Point2f unitDirection = new(direction.X / norm, direction.Y / norm);

                double dist1 = GeometryUtils.Distance(line.P1, face.Center);
                double dist2 = GeometryUtils.Distance(line.P2, face.Center);
                Point2f farthest = dist1 > dist2 ? line.P1 : line.P2;

                Point2f v = farthest - face.Center;
                float dot = v.X * unitDirection.X + v.Y * unitDirection.Y;

                Point foot = GeometryUtils.FindFootOfPerpendicular(line, (Point)face.Center);

                Point2f intersection = dot >= 0
                    ? new Point2f(
                        foot.X + unitDirection.X * (float)(face.Radius * 0.75),
                        foot.Y + unitDirection.Y * (float)(face.Radius * 0.75))
                    : new Point2f(
                        foot.X - unitDirection.X * (float)(face.Radius * 0.75),
                        foot.Y - unitDirection.Y * (float)(face.Radius * 0.75));

                handTips.Add((Point)intersection);
            }

            if (handTips.Count == 0)
                throw new GaugeReadingProcessException(GaugeErrorCode.GaugeHandNotFound);

            Point avgTip = GeometryUtils.GetCenterPoint(handTips);
            return new LineSegmentPoint((Point)face.Center, avgTip);
        }

        private static Mat GetPreprocessedImageForHandDetection(Mat image, CircleSegment face)
        {
            Mat blurred = new();
            Cv2.GaussianBlur(image, blurred, new Size(7, 7), 0);

            Mat edges = new();
            Cv2.Canny(blurred, edges, 100, 300);

            // mask to focus on gauge interior
            Mat mask = new(edges.Size(), MatType.CV_8UC1, Scalar.Black);
            Cv2.Circle(mask, (Point)face.Center, (int)(face.Radius * 0.7), Scalar.White, -1);

            Mat masked = new();
            edges.CopyTo(masked, mask);

            return masked;
        }
    }
}
