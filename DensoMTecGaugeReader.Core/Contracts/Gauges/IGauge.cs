namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Represents a general gauge (analog or digital).
    /// </summary>
    public interface IGauge
    {
        string Id { get; }
        string Unit { get; }
        Models.MeasurementResult ReadValue();
    }
}