using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoMTecGaugeReader.Core.Common
{
    public class GeometryUtils
    {
        public static OpenCvSharp.Point GetMiddlePoint(OpenCvSharp.Point point1, OpenCvSharp.Point point2)
        {
            double middlePointX = (point1.X + point2.X) / 2;
            double middlePointY = (point1.Y + point2.Y) / 2;
            return new OpenCvSharp.Point(middlePointX, middlePointY);

        }

        public static OpenCvSharp.Point GetCenterPoint(List<OpenCvSharp.Point> points)
        {
            double centerPointX = points.Sum(p => (double)(p.X)) / points.Count;
            double centerPointY = points.Sum(p => (double)(p.Y)) / points.Count;
            return new OpenCvSharp.Point(centerPointX, centerPointY);
        }

        public static LineSegmentPoint FitLineRansac(List<OpenCvSharp.Point> points)
        {
            Line2D line2d = Cv2.FitLine(points, DistanceTypes.L2, 0, 0.01, 0.01);

            float vx = (float)line2d.Vx;
            float vy = (float)line2d.Vy;
            float x1 = (float)line2d.X1;
            float y1 = (float)line2d.Y1;

            Point2f pointOnLine = new Point2f(x1, y1);
            Point2f direction = new Point2f(vx, vy);

            OpenCvSharp.Point pointStart = new OpenCvSharp.Point(
                (int)(pointOnLine.X - direction.X * 1000),
                (int)(pointOnLine.Y - direction.Y * 1000));

            OpenCvSharp.Point pointEnd = new OpenCvSharp.Point(
                (int)(pointOnLine.X + direction.X * 1000),
                (int)(pointOnLine.Y + direction.Y * 1000));

            return new LineSegmentPoint(pointStart, pointEnd);
        }

        public static bool IsLineInsideCircle(LineSegmentPoint line, CircleSegment circle, double thresholdRatio)
        {
            var firstPoint = line.P1;
            double distanceFirstPointToCenter = GetDistance(firstPoint, (OpenCvSharp.Point)circle.Center);
            if (distanceFirstPointToCenter > (double)circle.Radius * thresholdRatio)
            {
                return false;
            }

            var secondPoint = line.P2;
            double distanceSecondPointToCenter = GetDistance(secondPoint, (OpenCvSharp.Point)circle.Center);
            if (distanceSecondPointToCenter > (double)circle.Radius * thresholdRatio)
            {
                return false;
            }
            return true;
        }

        public static double GetDistance(OpenCvSharp.Point point1, OpenCvSharp.Point point2)
        {
            return Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));
        }

        public static bool IsBoxNearby(
            Rect box1,
            Rect box2,
            double threshold)
        {
            double distance = GetDistance(box1.TopLeft, box2.TopLeft);
            return distance <= threshold;
        }

        public static OpenCvSharp.Rect GetEnclosingBoundingBox(List<OpenCvSharp.Rect> boundingBoxes)
        {
            if (boundingBoxes == null || boundingBoxes.Count == 0)
            {
                throw new Exception("No bounding box found");
            }

            int minX = boundingBoxes[0].X;
            int minY = boundingBoxes[0].Y;
            int maxX = boundingBoxes[0].X + boundingBoxes[0].Width;
            int maxY = boundingBoxes[0].Y + boundingBoxes[0].Height;

            // Iterate through all bounding boxes to find the min and max coordinates
            foreach (var box in boundingBoxes)
            {
                minX = Math.Min(minX, box.X);
                minY = Math.Min(minY, box.Y);
                maxX = Math.Max(maxX, box.X + box.Width);
                maxY = Math.Max(maxY, box.Y + box.Height);
            }

            int enclosingWidth = maxX - minX;
            int enclosingHeight = maxY - minY;

            return new OpenCvSharp.Rect(minX, minY, enclosingWidth, enclosingHeight);
        }

        public static OpenCvSharp.Point GetRectangleCenter(OpenCvSharp.Rect rectangle)
        {
            double middlePointX = (rectangle.TopLeft.X + rectangle.BottomRight.X) / 2;
            double middlePointY = (rectangle.TopLeft.Y + rectangle.BottomRight.Y) / 2;
            return new OpenCvSharp.Point(middlePointX, middlePointY);
        }

        public static bool AreRectanglesAlmostEqual(OpenCvSharp.Rect rectangle1, OpenCvSharp.Rect rectangle2, double threshold)
        {
            return Math.Abs(rectangle1.X - rectangle2.X) <= threshold &&
               Math.Abs(rectangle1.Y - rectangle2.Y) <= threshold &&
               Math.Abs(rectangle1.Width - rectangle2.Width) <= threshold &&
               Math.Abs(rectangle1.Height - rectangle2.Height) <= threshold;
        }

        public static bool IsBoxInsideBiggerBox(OpenCvSharp.Rect boundingBox1, OpenCvSharp.Rect boundingBox2)
        {
            bool isBox1InsideBox2 = boundingBox1.TopLeft.X > boundingBox2.TopLeft.X &&
                boundingBox1.TopLeft.Y > boundingBox2.TopLeft.Y &&
                boundingBox1.BottomRight.X < boundingBox2.BottomRight.X &&
                boundingBox1.BottomRight.Y < boundingBox2.BottomRight.Y;

            bool isBox2InsideBox1 = boundingBox2.TopLeft.X > boundingBox1.TopLeft.X &&
                boundingBox2.TopLeft.Y > boundingBox1.TopLeft.Y &&
                boundingBox2.BottomRight.X < boundingBox1.BottomRight.X &&
                boundingBox2.BottomRight.Y < boundingBox1.BottomRight.Y;

            return isBox1InsideBox2 || isBox2InsideBox1;
        }

        public static OpenCvSharp.Point FindFootOfPerpendicular(LineSegmentPoint line, OpenCvSharp.Point point)
        {
            // Vector from P1 to P2 (the direction of the line)
            Point2f lineVec = new Point2f(line.P2.X - line.P1.X, line.P2.Y - line.P1.Y);

            // Vector from P1 to the given point
            Point2f pointVec = new Point2f(point.X - line.P1.X, point.Y - line.P1.Y);

            // Dot product of pointVec and lineVec
            float dotProduct = pointVec.X * lineVec.X + pointVec.Y * lineVec.Y;

            // Square of the magnitude of lineVec
            float lineLengthSquared = lineVec.X * lineVec.X + lineVec.Y * lineVec.Y;

            // Parameter 't' to find the projection on the line (normalized position along the line)
            float t = dotProduct / lineLengthSquared;

            // Clamp 't' to ensure the projected point lies on the segment (0 <= t <= 1)
            t = Math.Max(0, Math.Min(1, t));

            // Find the closest point using the parameter 't'
            float closestX = line.P1.X + t * lineVec.X;
            float closestY = line.P1.Y + t * lineVec.Y;

            return new OpenCvSharp.Point((int)closestX, (int)closestY);
        }

        public static OpenCvSharp.Point FindMirrorPoint(OpenCvSharp.Point point, OpenCvSharp.Point mirrorCenter)
        {
            int mirroredX = 2 * mirrorCenter.X - point.X;
            int mirroredY = 2 * mirrorCenter.Y - point.Y;

            return new OpenCvSharp.Point(mirroredX, mirroredY);
        }

        public static double CalculateAngle(LineSegmentPoint line1, LineSegmentPoint line2)
        {
            // Convert LineSegmentPoints to vectors.
            var vector1 = new OpenCvSharp.Point(line1.P2.X - line1.P1.X, line1.P2.Y - line1.P1.Y);
            var vector2 = new OpenCvSharp.Point(line2.P2.X - line2.P1.X, line2.P2.Y - line2.P1.Y);

            // Calculate dot product.
            double dotProduct = (vector1.X * vector2.X) + (vector1.Y * vector2.Y);

            // Calculate magnitudes of vectors.
            double magnitude1 = Math.Sqrt(vector1.X * vector1.X + vector1.Y * vector1.Y);
            double magnitude2 = Math.Sqrt(vector2.X * vector2.X + vector2.Y * vector2.Y);

            // Calculate the cosine of the angle and clamp it.
            double cosTheta = dotProduct / (magnitude1 * magnitude2);
            cosTheta = Math.Max(-1.0, Math.Min(1.0, cosTheta));

            // Compute the base angle (in radians) using arccos.
            double angleRadians = Math.Acos(cosTheta);

            // Determine the sign of the angle via the cross product.
            double crossProduct = vector1.X * vector2.Y - vector1.Y * vector2.X;

            // If the cross product is negative, adjust to get the reflex angle.
            if (crossProduct < 0)
            {
                angleRadians = 2 * Math.PI - angleRadians;
            }

            // Convert the angle to degrees.
            double angleDegrees = angleRadians * (180.0 / Math.PI);
            return angleDegrees;
        }

        public static LineSegmentPoint GetLineFromLine(LineSegmentPoint line, double angle, double length)
        {
            // Convert angle to radians
            double angleRad = angle * Math.PI / 180.0;

            // Extract start and end points
            OpenCvSharp.Point p1 = line.P1;
            OpenCvSharp.Point p2 = line.P2;

            // Calculate the direction of the reference line
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            double refAngle = Math.Atan2(dy, dx);

            // New line angle relative to the reference line
            double newAngle = refAngle + angleRad;

            // Calculate new endpoint using the new angle
            int x3 = (int)(p1.X + length * Math.Cos(newAngle));
            int y3 = (int)(p1.Y + length * Math.Sin(newAngle));

            return new LineSegmentPoint(p1, new OpenCvSharp.Point(x3, y3));
        }

        public static LineSegmentPoint GetAverageLine(List<LineSegmentPoint> lines, double length)
        {
            double sumX = 0, sumY = 0;

            // Compute the average unit vector
            foreach (var line in lines)
            {
                double dx = line.P2.X - line.P1.X;
                double dy = line.P2.Y - line.P1.Y;
                double magnitude = Math.Sqrt(dx * dx + dy * dy);

                if (magnitude > 0)
                {
                    sumX += dx / magnitude; // Normalize and sum
                    sumY += dy / magnitude;
                }
            }

            // Normalize the average vector
            double avgMagnitude = Math.Sqrt(sumX * sumX + sumY * sumY);
            double avgDX = (sumX / avgMagnitude) * length;
            double avgDY = (sumY / avgMagnitude) * length;

            OpenCvSharp.Point commonPoint = lines[0].P1;
            OpenCvSharp.Point avgEnd = new OpenCvSharp.Point((int)(commonPoint.X + avgDX), (int)(commonPoint.Y + avgDY));

            return new LineSegmentPoint(commonPoint, avgEnd);
        }
    }
}
