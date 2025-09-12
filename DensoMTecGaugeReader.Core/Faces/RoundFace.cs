using OpenCvSharp;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Faces
{
    /// <summary>
    /// Implementation for round gauge faces.
    /// Responsible for normalizing circular images and detecting hands.
    /// </summary>
    public class RoundFace : IGaugeFace
    {
        public GaugeFaceType FaceType => GaugeFaceType.Round;

        public Mat Normalize(Mat raw)
        {
            // TODO: Replace with actual circle detection + warp
            // Example: detect circle via HoughCircles, crop & warp to normalize
            return raw.Clone();
        }

        public GaugeHandInfo? ExtractHand(Mat normalizedImage)
        {
            // TODO: Replace with HoughLines / contour logic
            // Placeholder: simulate detected hand at 45°
            return new GaugeHandInfo(
                startX: 100,
                startY: 100,
                endX: 200,
                endY: 200,
                angle: 45,
                length: 100,
                confidence: 0.9
            );
        }
    }
}