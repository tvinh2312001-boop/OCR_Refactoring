namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Geometry information of a detected gauge face.
    /// </summary>
    public class GaugeFaceInfo
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }
}
