using System;
using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;
using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Detectors
{
    /// <summary>
    /// Detects a round gauge face using HoughCircles.
    /// </summary>
    public class RoundFaceDetector : IGaugeFaceDetector
    {
        public GaugeFaceInfo DetectFace(string imagePath)
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (src.Empty())
                throw new ArgumentException("Image not found or cannot be read.", nameof(imagePath));

            using var gray = new Mat();
            using var blur = new Mat();

            Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.GaussianBlur(gray, blur, new Size(9, 9), 2);
            Cv2.ImShow("Blur find face",blur );
            var circles = Cv2.HoughCircles(
                blur,
                HoughModes.Gradient,
                dp: 1.2,
                minDist: Math.Min(src.Rows, src.Cols) / 8.0,
                param1: 120,
                param2: 50,
                minRadius: (int)(Math.Min(src.Rows, src.Cols) * 0.15),
                maxRadius: (int)(Math.Min(src.Rows, src.Cols) * 0.48)
            );

            if (circles == null || circles.Length == 0)
                throw new InvalidOperationException("No round gauge face detected.");

            var imgCenter = new Point(src.Cols / 2, src.Rows / 2);
            CircleSegment best = circles[0];
            double bestScore = double.MaxValue;

            foreach (var c in circles)
            {
                var cpt = new Point((int)c.Center.X, (int)c.Center.Y);
                double dc = GeometryUtils.GetDistance(cpt, imgCenter);
                // prefer centered & large
                double score = dc - c.Radius * 0.25;
                if (score < bestScore)
                {
                    bestScore = score;
                    best = c;
                    Cv2.Circle(src, cpt, 3, Scalar.Red, -1);
                    Cv2.Circle(src, cpt, (int)c.Radius, Scalar.Green, 2);
                    Cv2.ImShow("Detected Circle", src);
                    Cv2.WaitKey(0);
                }
            }

            return new GaugeFaceInfo
            {
                CenterX = best.Center.X,
                CenterY = best.Center.Y,
                Radius = best.Radius,
                Width = src.Cols,
                Height = src.Rows
            };
        }
    }
}
