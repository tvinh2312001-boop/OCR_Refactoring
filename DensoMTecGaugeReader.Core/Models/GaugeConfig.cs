using DensoMTecGaugeReader.Core.Common.Enums;

namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Configuration for a specific gauge instance.
    /// Defines gauge type, face type, and measurement range.
    /// </summary>
    public class GaugeConfig
    {
        public string GaugeId { get; set; } = string.Empty;

        public GaugeType GaugeType { get; set; }
        public GaugeFaceType FaceType { get; set; }

        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        public double StartAngle { get; set; }
        public double EndAngle { get; set; }

        public string Unit { get; set; } = string.Empty;
    }
}