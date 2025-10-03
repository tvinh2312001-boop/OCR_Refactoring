using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Contracts.Gauges
{
    /// <summary>
    /// Detects the hand/needle of a gauge.
    /// </summary>
    public interface IGaugeHandDetector
    {
        GaugeHandInfo DetectHand(string imagePath, GaugeFaceInfo faceInfo);
    }
}