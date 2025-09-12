namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Result of a gauge measurement.
    /// </summary>
    public class MeasurementResult
    {
        public double HandAngle { get; set; }
        public double ScaleAngle { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; } = string.Empty;

        public MeasurementResult(double handAngle, double scaleAngle, double value, string unit)
        {
            HandAngle = handAngle;
            ScaleAngle = scaleAngle;
            Value = value;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{Value:F2} {Unit} (HandAngle={HandAngle:F2}°, ScaleAngle={ScaleAngle:F2}°)";
        }
    }
}