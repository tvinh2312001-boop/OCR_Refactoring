using OpenCvSharp;

namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Represents the detected hand/needle of a gauge.
    /// </summary>
    public class GaugeHandInfo
    {
        public LineSegmentPoint Line { get; set; }
        public double Angle { get; set; }

        public GaugeHandInfo(LineSegmentPoint line, double angle)
        {
            Line = line;
            Angle = angle;
        }
    }
}