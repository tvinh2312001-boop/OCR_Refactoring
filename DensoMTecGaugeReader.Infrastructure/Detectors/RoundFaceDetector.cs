using System.Linq;
using OpenCvSharp;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Common.Enums;
using DensoMTecGaugeReader.Core.Common.Errors;

namespace DensoMTecGaugeReader.Infrastructure.ImageProcessing
{
    /// <summary>
    /// Round face detector implementation using OpenCV.
    /// Implements IFaceDetector and returns GaugeFaceInfo.
    /// </summary>
    public class RoundFaceDetector : IGaugeFaceDetector
    {
        /// <summary>
        /// Detect round gauge face from an input image.
        /// </summary>
        public GaugeFaceInfo Detect(Mat image)
        {
            var preprocessed = Preprocess(image);

            int maxRadius = GetGaugeFaceMaxRadius(preprocessed);
            int minRadius = GetGaugeFaceMinRadius(preprocessed);

            var circles = Cv2.HoughCircles(
                preprocessed,
                HoughModes.Gradient,
                dp: 1,
                minDist: preprocessed.Rows / 8,
                param1: 100,
                param2: 100,
                minRadius: minRadius,
                maxRadius: maxRadius
            );

            if (circles.Length == 0)
                throw new GaugeException(GaugeErrorCode.GaugeFaceNotFound, "No round face detected");

            var circle = circles.OrderByDescending(c => c.Radius).First();

            return new GaugeFaceInfo
            {
                FaceType = GaugeFaceType.Round,
                Center = ((int)circle.Center.X, (int)circle.Center.Y),
                Radius = circle.Radius
            };
        }

        /// <summary>
        /// Preprocess image for face detection (grayscale, blur, Canny).
        /// </summary>
        private static Mat Preprocess(Mat image)
        {
            Mat grayImage = new();
            Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

            Mat bluredImage = new();
            Cv2.GaussianBlur(grayImage, bluredImage, new Size(11, 11), 0);

            Mat canny = new();
            Cv2.Canny(bluredImage, canny, 100, 300);

            return canny;
        }

        private static int GetGaugeFaceMinRadius(Mat image)
        {
            int smallerSize = System.Math.Min(image.Width, image.Height);
            return (int)(smallerSize * 0.05);
        }

        private static int GetGaugeFaceMaxRadius(Mat image)
        {
            int smallerSize = System.Math.Min(image.Width, image.Height);
            return (int)(smallerSize * 0.5);
        }
    }
}
