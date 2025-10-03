namespace DensoMTecGaugeReader.Infrastructure.Models
{
    /// <summary>
    /// Infra version of AnalogGaugeNumber with OpenCV Rect.
    /// This will be mapped back to Core model.
    /// </summary>
    public class AnalogGaugeNumber
    {
        public string Value { get; set; } = string.Empty;
        public double X { get; set; }
        public double Y { get; set; }
    }
}