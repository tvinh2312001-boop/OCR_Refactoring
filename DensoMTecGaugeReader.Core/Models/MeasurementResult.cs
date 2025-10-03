namespace DensoMTecGaugeReader.Core.Models
{
    /// <summary>
    /// Final measurement result of a gauge reading.
    /// </summary>
    public class MeasurementResult
    {
        public double RawAngle { get; set; }
        public double Value { get; set; }
        public string Unit { get; set; }
    }

}