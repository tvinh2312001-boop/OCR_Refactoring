using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Detects the face of a gauge from an image source.
    /// </summary>
    public interface IGaugeFaceDetector
    {
        GaugeFaceInfo DetectFace(string imagePath);
    }
}