using OpenCvSharp;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Infrastructure.Utils;

namespace DensoMTecGaugeReader.Infrastructure.ImageProcessing
{
    /// <summary>
    /// Provides methods for detecting the baseline (reference axis) of a round gauge face.
    /// Baseline is determined by analyzing gauge numbers and their relative angles.
    /// </summary>
    public static class BaseLineDetector
    {
        /// <summary>
        /// Detect baseline line of a gauge face from detected numbers.
        /// </summary>
        public static LineSegmentPoint DetectBaseLine(List<AnalogGaugeNumber> numbers, CircleSegment face)
        {
            const double angleThreshold = 5;
            numbers = FilterValidNumbers(numbers);

            List<Tuple<AnalogGaugeNumber, AnalogGaugeNumber>> validPairs = new();

            // Pair numbers and check consistency with expected angle
            for (int i = 0; i < numbers.Count; i++)
            {
                var num1 = numbers[i];
                var lineToCenter1 = new LineSegmentPoint(
                    (Point)face.Center,
                    GeometryUtils.GetRectangleCenter(num1.BoundingBox)
                );
                var defaultAngle1 = GetDefaultAngleByValue(num1.Value);

                for (int j = i + 1; j < numbers.Count; j++)
                {
                    var num2 = numbers[j];
                    var lineToCenter2 = new LineSegmentPoint(
                        (Point)face.Center,
                        GeometryUtils.GetRectangleCenter(num2.BoundingBox)
                    );
                    var defaultAngle2 = GetDefaultAngleByValue(num2.Value);

                    var realAngle = GeometryUtils.CalculateAngle(lineToCenter2, lineToCenter1);
                    var expectedAngle = Math.Abs(defaultAngle2 - defaultAngle1);

                    if ((expectedAngle > realAngle - angleThreshold && expectedAngle < realAngle + angleThreshold) ||
                        (360 - expectedAngle > realAngle - angleThreshold && 360 - expectedAngle < realAngle + angleThreshold))
                    {
                        validPairs.Add(new Tuple<AnalogGaugeNumber, AnalogGaugeNumber>(num1, num2));
                    }
                }
            }

            if (validPairs.Count == 0)
                throw new GaugeReadingProcessException(GaugeErrorCode.GaugeBaseLineNotFound);

            // Build candidate baselines
            List<LineSegmentPoint> candidateLines = new();
            foreach (var pair in validPairs)
            {
                candidateLines.Add(
                    GetBaseLineFromNumber(pair.Item1, (Point)face.Center)
                );
            }

            // Average baseline
            double baseLineLength = face.Radius;
            return GeometryUtils.GetAverageLine(candidateLines, baseLineLength);
        }

        /// <summary>
        /// Build baseline from a number value and face center.
        /// </summary>
        private static LineSegmentPoint GetBaseLineFromNumber(AnalogGaugeNumber number, Point faceCenter)
        {
            var lineToCenter = new LineSegmentPoint(
                faceCenter,
                GeometryUtils.GetRectangleCenter(number.BoundingBox)
            );

            double defaultAngle = GetDefaultAngleByValue(number.Value);

            return GeometryUtils.GetLineFromLine(lineToCenter, -defaultAngle, 100);
        }

        /// <summary>
        /// Default reference angle for a given gauge number.
        /// This mapping should be configured depending on gauge type.
        /// </summary>
        private static double GetDefaultAngleByValue(string value)
        {
            return value switch
            {
                "02" or "2" => 53,
                "04" or "4" => 111,
                "06" or "6" => 164,
                "08" or "8" => 212,
                "1"         => 265,
                _           => 0
            };
        }

        /// <summary>
        /// Filter out valid numbers for baseline detection.
        /// </summary>
        private static List<AnalogGaugeNumber> FilterValidNumbers(List<AnalogGaugeNumber> numbers)
        {
            HashSet<string> validNumbers = new() { "02", "04", "06", "08", "1", "2", "4", "6", "8" };
            return numbers.Where(x => validNumbers.Contains(x.Value)).ToList();
        }
    }
}
