namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Represents the final measurement result of a gauge.
    /// </summary>
    public class MeasurementResult
    {
        public double RawAngle { get; set; }
        public double NormalizedAngle { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }

        public MeasurementResult(double rawAngle, double normalizedAngle, double value, string unit)
        {
            RawAngle = rawAngle;
            NormalizedAngle = normalizedAngle;
            Value = value;
            Unit = unit;
        }

        public override string ToString()
        {
            return $"{Value:F2} {Unit} (Angle={RawAngle:F1}° → Normalized={NormalizedAngle:F1}°)";
        }
    }
}