using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Contracts
{
    /// <summary>
    /// Base interface for all gauges (Pressure, Thermo, Compound, Digital).
    /// </summary>
    public interface IGauge
    {
        string Name { get; }
        MeasurementResult Measure(GaugeFaceInfo face, GaugeHandInfo? hand);
    }
}