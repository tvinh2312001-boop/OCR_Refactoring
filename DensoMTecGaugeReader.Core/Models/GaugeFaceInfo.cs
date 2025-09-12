namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Information about the detected gauge face (center, radius, etc.).
    /// </summary>
    public class GaugeFaceInfo
    {
        public double CenterX { get; set; }
        public double CenterY { get; set; }
        public double Radius { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public GaugeConfig Config { get; set; }

        public GaugeFaceInfo(double centerX, double centerY, double radius, GaugeConfig config)
        {
            CenterX = centerX;
            CenterY = centerY;
            Radius = radius;
            Config = config;
        }

        public GaugeFaceInfo(double centerX, double centerY, double width, double height, GaugeConfig config)
        {
            CenterX = centerX;
            CenterY = centerY;
            Width = width;
            Height = height;
            Config = config;
        }
    }
}