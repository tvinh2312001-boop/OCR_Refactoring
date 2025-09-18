using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Interface for detecting gauge hands (needles).
    /// Implementations will use computer vision to locate the hand
    /// and return its geometric information.
    /// </summary>
    public interface IGaugeHandDetector
    {
        GaugeHandInfo Detect(GaugeFaceInfo faceInfo);
    }
}