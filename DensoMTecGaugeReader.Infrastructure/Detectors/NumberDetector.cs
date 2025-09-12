using OpenCvSharp;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Infrastructure.Utils;

namespace DensoMTecGaugeReader.Infrastructure.ImageProcessing
{
    /// <summary>
    /// Detects and recognizes numbers on gauge faces using contour detection and OCR.
    /// </summary>
    public class NumberDetector
    {
        private readonly INumberRecognitionService _numberRecognitionService;

        public NumberDetector(INumberRecognitionService numberRecognitionService)
        {
            _numberRecognitionService = numberRecognitionService;
        }

        /// <summary>
        /// Detect numbers on a gauge face.
        /// </summary>
        public List<AnalogGaugeNumber> DetectNumbers(Mat preprocessedImage, CircleSegment gaugeFace)
        {
            Cv2.FindContours(
                preprocessedImage,
                out Point[][] contours,
                out HierarchyIndex[] hierarchy,
                RetrievalModes.List,
                ContourApproximationModes.ApproxSimple
            );

            // filter bounding boxes
            List<(Rect BoundingBox, Point[][] Contour)> validContours = new();
            foreach (var contour in contours)
            {
                Rect boundingBox = Cv2.BoundingRect(contour);
                if (!IsNumberBoundingBoxValid(boundingBox, gaugeFace))
                    continue;

                validContours.Add((boundingBox, new[] { contour }));
            }

            // group nearby contours
            const double groupDistanceRatio = 0.15;
            var groupedContours = GroupContours(validContours, gaugeFace.Radius * groupDistanceRatio);

            // recognize numbers
            List<AnalogGaugeNumber> gaugeNumbers = new();
            foreach (var group in groupedContours)
            {
                if (group.Count == 0) continue;
                gaugeNumbers.Add(GetGaugeNumber(group, preprocessedImage));
            }

            return gaugeNumbers;
        }

        /// <summary>
        /// Extract one gauge number from grouped contours using OCR service.
        /// </summary>
        private AnalogGaugeNumber GetGaugeNumber(
            List<(Rect BoundingBox, Point[][] Contour)> contourGroup,
            Mat preprocessedImage)
        {
            AnalogGaugeNumber result = new();

            // remove duplicates
            contourGroup = RemoveContoursWithDuplicatedBoundingBox(contourGroup);

            var groupedBoundingBox = contourGroup.Select(x => x.BoundingBox).ToList();
            result.BoundingBox = GeometryUtils.GetEnclosingBoundingBox(groupedBoundingBox);

            // crop and pad region
            Mat numberRegion = new(preprocessedImage, result.BoundingBox);
            int borderSize = 4;
            Mat paddedRegion = new();
            Cv2.CopyMakeBorder(
                numberRegion,
                paddedRegion,
                borderSize, borderSize, borderSize, borderSize,
                BorderTypes.Constant,
                Scalar.Black
            );

            // recognize value
            result.Value = _numberRecognitionService.RecognizeNumber(paddedRegion);
            return result;
        }

        private static List<(Rect BoundingBox, Point[][] Contour)> RemoveContoursWithDuplicatedBoundingBox(
            List<(Rect BoundingBox, Point[][] Contour)> contourGroup)
        {
            List<(Rect BoundingBox, Point[][] Contour)> nonDuplicated = new();
            HashSet<int> visited = new();

            for (int i = 0; i < contourGroup.Count; i++)
            {
                if (visited.Contains(i)) continue;

                Rect currentBox = contourGroup[i].BoundingBox;

                for (int j = 0; j < contourGroup.Count; j++)
                {
                    if (visited.Contains(j) || i == j) continue;

                    if (GeometryUtils.AreRectanglesAlmostEqual(currentBox, contourGroup[j].BoundingBox, 5))
                        visited.Add(j);
                }

                nonDuplicated.Add(contourGroup[i]);
            }

            return nonDuplicated;
        }

        private static bool IsNumberBoundingBoxValid(Rect boundingBox, CircleSegment gaugeFace)
        {
            double meterArea = Math.PI * Math.Pow(gaugeFace.Radius, 2);

            const double maxWidthRatio = 0.03;
            const double maxHeightRatio = 0.03;
            const double maxAreaRatio = 0.00125;
            const double minDistanceRatio = 0.55;
            const double maxDistanceRatio = 0.95;

            if (boundingBox.Width < gaugeFace.Radius * maxWidthRatio) return false;
            if (boundingBox.Height < gaugeFace.Radius * maxHeightRatio) return false;
            if (boundingBox.Width * boundingBox.Height < meterArea * maxAreaRatio) return false;

            double distance = GeometryUtils.GetDistance(boundingBox.TopLeft, (Point)gaugeFace.Center);
            if (distance < gaugeFace.Radius * minDistanceRatio ||
                distance > gaugeFace.Radius * maxDistanceRatio)
                return false;

            return true;
        }

        private static List<List<(Rect BoundingBox, Point[][] Contour)>> GroupContours(
            List<(Rect BoundingBox, Point[][] Contour)> contours,
            double distanceThreshold)
        {
            List<List<(Rect BoundingBox, Point[][] Contour)>> groupedContours = new();
            HashSet<int> groupedIds = new();

            for (int i = 0; i < contours.Count; i++)
            {
                if (groupedIds.Contains(i)) continue;

                var current = contours[i];
                List<(Rect BoundingBox, Point[][] Contour)> group = new() { current };
                groupedIds.Add(i);

                for (int j = 0; j < contours.Count; j++)
                {
                    if (groupedIds.Contains(j) || i == j) continue;

                    if (GeometryUtils.IsBoxNearby(current.BoundingBox, contours[j].BoundingBox, distanceThreshold))
                    {
                        group.Add(contours[j]);
                        groupedIds.Add(j);
                    }
                }

                group = group.OrderBy(x => x.BoundingBox.X).ToList();
                if (group.Count > 0) groupedContours.Add(group);
            }

            return groupedContours;
        }
    }
}