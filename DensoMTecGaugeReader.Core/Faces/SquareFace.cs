using OpenCvSharp;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Faces
{
    /// <summary>
    /// Implementation for square/rectangular gauge faces.
    /// Responsible for normalizing rectangular images and detecting hands.
    /// </summary>
    public class SquareFace : IGaugeFace
    {
        public GaugeFaceType FaceType => GaugeFaceType.Square;

        public Mat Normalize(Mat raw)
        {
            // TODO: Replace with actual square/rectangle detection
            // Example: detect bounding box, crop and align
            return raw.Clone();
        }

        public GaugeHandInfo? ExtractHand(Mat normalizedImage)
        {
            // TODO: Replace with detection logic for square faces
            // Placeholder: simulate detected hand at 30°
            return new GaugeHandInfo(
                startX: 80,
                startY: 80,
                endX: 160,
                endY: 200,
                angle: 30,
                length: 120,
                confidence: 0.85
            );
        }
    }
}