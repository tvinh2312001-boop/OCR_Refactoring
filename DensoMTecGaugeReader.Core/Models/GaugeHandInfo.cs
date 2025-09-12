namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Information about the detected gauge hand (needle/pointer).
    /// </summary>
    public class GaugeHandInfo
    {
        public double StartX { get; set; }
        public double StartY { get; set; }

        public double EndX { get; set; }
        public double EndY { get; set; }

        public double Angle { get; set; }

        public double Length { get; set; }

        public double Confidence { get; set; }

        public GaugeHandInfo(double startX, double startY, double endX, double endY, double angle, double length, double confidence)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            Angle = angle;
            Length = length;
            Confidence = confidence;
        }
    }
}
