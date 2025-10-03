namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Information of a detected gauge hand/needle.
    /// </summary>
    public class GaugeHandInfo
    {
        public double StartX { get; set; }
        public double StartY { get; set; }
        public double EndX { get; set; }
        public double EndY { get; set; }
        public double Angle { get; set; }
    }
}