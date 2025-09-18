using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Interface for detecting gauge faces.
    /// Each implementation handles a specific face type (round, square, digital).
    /// </summary>
    public interface IGaugeFaceDetector
    {
        /// <summary>
        /// Detect gauge face information from an input image.
        /// </summary>
        /// <param name="image">Input image</param>
        /// <returns>Detected face info</returns>
        GaugeFaceInfo Detect(OpenCvSharp.Mat image);
    }
}