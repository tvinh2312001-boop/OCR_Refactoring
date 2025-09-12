using OpenCvSharp;
using DensoMTecGaugeReader.Core.Exceptions;

namespace DensoMTecGaugeReader.Infrastructure.ImageProcessing
{
    /// <summary>
    /// Provides face detection utilities for round gauges.
    /// Handles preprocessing and circle detection with OpenCV.
    /// </summary>
    public static class RoundFaceDetector
    {
        /// <summary>
        /// Detect round gauge face from a preprocessed image using HoughCircles.
        /// </summary>
        public static CircleSegment DetectGaugeFace(Mat preprocessedImage)
        {
            int maxRadius = GetGaugeFaceMaxRadius(preprocessedImage);
            int minRadius = GetGaugeFaceMinRadius(preprocessedImage);

            var circles = Cv2.HoughCircles(
                preprocessedImage,
                HoughModes.Gradient,
                dp: 1,
                minDist: preprocessedImage.Rows / 8,
                param1: 100,
                param2: 100,
                minRadius: minRadius,
                maxRadius: maxRadius
            );

            if (circles.Length == 0)
                throw new GaugeReadingProcessException(GaugeErrorCode.GaugeFaceNotFound);

            // Return the largest circle
            return circles.OrderByDescending(c => c.Radius).First();
        }

        /// <summary>
        /// Preprocess image for face detection (grayscale, blur, Canny).
        /// </summary>
        public static Mat GetPreprocessedImageForFaceDetection(Mat image)
        {
            Mat grayImage = new();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            Mat bluredImage = new();
            Cv2.GaussianBlur(grayImage, bluredImage, new Size(11, 11), 0);

            Mat canny = new();
            Cv2.Canny(bluredImage, canny, 100, 300);

            return canny;
        }

        /// <summary>
        /// Compute new face circle when image is resized.
        /// </summary>
        public static CircleSegment GetGaugeFaceInNewImage(CircleSegment original, double resizedRatio)
        {
            return new CircleSegment(
                new Point(
                    (float)(original.Center.X * resizedRatio),
                    (float)(original.Center.Y * resizedRatio)
                ),
                (float)(original.Radius * resizedRatio)
            );
        }

        /// <summary>
        /// Compute resize ratio based on radius.
        /// Default target radius is 110 px.
        /// </summary>
        public static double GetResizeRatio(float radius)
        {
            const double defaultRadius = 110.0;
            return defaultRadius / radius;
        }

        private static int GetGaugeFaceMinRadius(Mat image)
        {
            int smallerSize = Math.Min(image.Width, image.Height);
            return (int)(smallerSize * 0.05);
        }

        private static int GetGaugeFaceMaxRadius(Mat image)
        {
            int smallerSize = Math.Min(image.Width, image.Height);
            return (int)(smallerSize * 0.5);
        }
    }
}