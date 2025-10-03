using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Infrastructure.Models;
using OpenCvSharp;

namespace DensoMTecGaugeReader.Infrastructure.Detectors
{
    /// <summary>
    /// Detects digit labels around the gauge rim via contour filtering.
    /// Optional OCR hook can be provided to read text from each ROI.
    /// </summary>
    public class NumberDetector
    {
        private readonly Func<Mat, string> _ocr;

        public NumberDetector(Func<Mat, string> ocr = null)
        {
            _ocr = ocr;
        }

        public List<AnalogGaugeNumber> DetectNumbers(string imagePath, GaugeFaceInfo face)
        {
            using var src = Cv2.ImRead(imagePath, ImreadModes.Color);
            if (src.Empty())
                throw new ArgumentException("Image not found or cannot be read.", nameof(imagePath));

            // ring mask: between rInner and rOuter
            using var maskInner = new Mat(src.Size(), MatType.CV_8UC1, Scalar.Black);
            using var maskOuter = new Mat(src.Size(), MatType.CV_8UC1, Scalar.Black);

            int rInner = (int)(face.Radius * 0.65);
            int rOuter = (int)(face.Radius * 1.10);

            var cpt = new Point((int)face.CenterX, (int)face.CenterY);
            Cv2.Circle(maskInner, cpt, rInner, Scalar.White, -1);
            Cv2.Circle(maskOuter, cpt, rOuter, Scalar.White, -1);

            using var ringMask = new Mat();
            Cv2.Subtract(maskOuter, maskInner, ringMask);

            using var ring = new Mat();
            Cv2.BitwiseAnd(src, src, ring, ringMask);

            using var gray = new Mat();
            using var thr = new Mat();
            Cv2.CvtColor(ring, gray, ColorConversionCodes.BGR2GRAY);
            Cv2.AdaptiveThreshold(gray, thr, 255, AdaptiveThresholdTypes.GaussianC, ThresholdTypes.BinaryInv, 31, 7);

            Cv2.FindContours(thr, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            // collect candidate boxes
            var boxes = new List<Rect>();
            foreach (var cnt in contours)
            {
                var rect = Cv2.BoundingRect(cnt);
                if (rect.Width < face.Radius * 0.04 || rect.Height < face.Radius * 0.04) continue;
                if (rect.Width > face.Radius * 0.40 || rect.Height > face.Radius * 0.40) continue;

                var center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
                double d = GeometryUtils.GetDistance(center, cpt);
                if (d < rInner || d > rOuter) continue;

                boxes.Add(rect);
            }

            // de-duplicate near-identical boxes (use utils)
            var deduped = DeduplicateBoxes(boxes, threshold: 3.0);

            var results = new List<AnalogGaugeNumber>();
            foreach (var rect in deduped)
            {
                string text = string.Empty;
                if (_ocr != null)
                {
                    using var roi = new Mat(gray, rect);
                    text = _ocr(roi) ?? string.Empty;
                    text = text.Trim();
                }

                double cx = rect.X + rect.Width / 2.0;
                double cy = rect.Y + rect.Height / 2.0;

                results.Add(new AnalogGaugeNumber
                {
                    Value = text,
                    X = cx,
                    Y = cy
                });
            }

            return results;
        }

        private static List<Rect> DeduplicateBoxes(List<Rect> boxes, double threshold)
        {
            var result = new List<Rect>();
            foreach (var box in boxes.OrderBy(b => b.X).ThenBy(b => b.Y))
            {
                bool exists = result.Any(r => GeometryUtils.AreRectanglesAlmostEqual(r, box, threshold));
                if (!exists) result.Add(box);
            }
            return result;
        }

        public static bool TryParseNumber(string s, out double value)
        {
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }
}
