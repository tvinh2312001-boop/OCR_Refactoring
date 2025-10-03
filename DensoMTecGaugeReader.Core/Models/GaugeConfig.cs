using DensoMTecGaugeReader.Core.Common.Enums;
using System.Collections.Generic;

namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Configuration for a gauge, including its range and angles.
    /// </summary>
    public class GaugeConfig
    {
        public string GaugeId { get; set; } = string.Empty;
        public GaugeType GaugeType { get; set; }
        public GaugeFaceType FaceType { get; set; }
        public ScaleType ScaleType { get; set; }
        public string Unit { get; set; } = string.Empty;

        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }
        public Dictionary<string, double> NumberToAngleMap { get; set; } = new();
    }
}