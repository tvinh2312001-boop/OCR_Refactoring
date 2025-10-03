using System;
using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;
using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Detectors
{
    /// <summary>
    /// Detects a square/rectangular gauge face via contour + polygon approximation.
    /// </summary>
    public class SquareFaceDetector : IGaugeFaceDetector
    {
        public GaugeFaceInfo DetectFace(string imagePath)
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (src.Empty())
                throw new ArgumentException("Image not found or cannot be read.", nameof(imagePath));

            using var gray = new Mat();
            using var blur = new Mat();
            using var edges = new Mat();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, blur, new Size(5, 5), 1.5);
            Cv2.Canny(blur, edges, 80, 160);

            Cv2.FindContours(edges, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);
            if (contours == null || contours.Length == 0)
                throw new InvalidOperationException("No contours found for square face detection.");

            var imgCenter = new Point(src.Cols / 2, src.Rows / 2);

            RotatedRect bestBox = default;
            double bestScore = double.MinValue;

            foreach (var cnt in contours)
            {
                if (Cv2.ContourArea(cnt) < (src.Rows * src.Cols) * 0.05) continue;

                var peri = Cv2.ArcLength(cnt, true);
                var approx = Cv2.ApproxPolyDP(cnt, 0.02 * peri, true);

                if (approx.Length >= 4)
                {
                    var box = Cv2.MinAreaRect(cnt);
                    var size = box.Size;
                    double area = size.Width * size.Height;

                    var center = new Point((int)box.Center.X, (int)box.Center.Y);
                    double dc = GeometryUtils.GetDistance(center, imgCenter);
                    double score = area - dc * 50.0;

                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestBox = box;
                    }
                }
            }

            if (bestScore == double.MinValue)
                throw new InvalidOperationException("No square-like face detected.");

            double pseudoRadius = Math.Min(bestBox.Size.Width, bestBox.Size.Height) / 2.0;

            return new GaugeFaceInfo
            {
                CenterX = bestBox.Center.X,
                CenterY = bestBox.Center.Y,
                Radius = pseudoRadius,
                Width = src.Cols,
                Height = src.Rows
            };
        }
    }
}
