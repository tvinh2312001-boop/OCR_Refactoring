using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Utils
{
    /// <summary>
    /// Provides geometry helper functions for gauge image processing.
    /// </summary>
    public static class GeometryUtils
    {
        public static Point GetRectangleCenter(Rect rect) =>
            new(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

        public static double CalculateAngle(LineSegmentPoint line1, LineSegmentPoint line2)
        {
            var v1 = new Point2f(line1.P2.X - line1.P1.X, line1.P2.Y - line1.P1.Y);
            var v2 = new Point2f(line2.P2.X - line2.P1.X, line2.P2.Y - line2.P1.Y);

            double dot = v1.X * v2.X + v1.Y * v2.Y;
            double mag1 = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y);
            double mag2 = Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y);

            return Math.Acos(dot / (mag1 * mag2)) * (180.0 / Math.PI);
        }

        public static double Distance(Point p1, Point2f p2) =>
            Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static Rect GetEnclosingBoundingBox(IEnumerable<Rect> rects)
        {
            int minX = rects.Min(r => r.X);
            int minY = rects.Min(r => r.Y);
            int maxX = rects.Max(r => r.X + r.Width);
            int maxY = rects.Max(r => r.Y + r.Height);

            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public static bool AreRectanglesAlmostEqual(Rect r1, Rect r2, int tolerance)
        {
            return Math.Abs(r1.X - r2.X) < tolerance &&
                   Math.Abs(r1.Y - r2.Y) < tolerance &&
                   Math.Abs(r1.Width - r2.Width) < tolerance &&
                   Math.Abs(r1.Height - r2.Height) < tolerance;
        }

        public static bool IsBoxNearby(Rect r1, Rect r2, double threshold)
        {
            double centerDist = Distance(GetRectangleCenter(r1), GetRectangleCenter(r2));
            return centerDist < threshold;
        }

        public static double Distance(Point p1, Point p2) =>
            Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));

        public static Point GetCenterPoint(List<Point> points)
        {
            int avgX = (int)points.Average(p => p.X);
            int avgY = (int)points.Average(p => p.Y);
            return new Point(avgX, avgY);
        }

        public static Point FindFootOfPerpendicular(LineSegmentPoint line, Point point)
        {
            double dx = line.P2.X - line.P1.X;
            double dy = line.P2.Y - line.P1.Y;
            double magSq = dx * dx + dy * dy;

            double u = ((point.X - line.P1.X) * dx + (point.Y - line.P1.Y) * dy) / magSq;
            int x = (int)(line.P1.X + u * dx);
            int y = (int)(line.P1.Y + u * dy);

            return new Point(x, y);
        }

        public static LineSegmentPoint GetAverageLine(List<LineSegmentPoint> lines, double length)
        {
            int avgX = (int)lines.Average(l => l.P2.X);
            int avgY = (int)lines.Average(l => l.P2.Y);
            return new LineSegmentPoint(lines.First().P1, new Point(avgX, avgY));
        }

        public static LineSegmentPoint GetLineFromLine(LineSegmentPoint baseLine, double angle, double length)
        {
            double rad = angle * Math.PI / 180.0;
            int newX = (int)(baseLine.P2.X + length * Math.Cos(rad));
            int newY = (int)(baseLine.P2.Y + length * Math.Sin(rad));
            return new LineSegmentPoint(baseLine.P1, new Point(newX, newY));
        }
    }
}