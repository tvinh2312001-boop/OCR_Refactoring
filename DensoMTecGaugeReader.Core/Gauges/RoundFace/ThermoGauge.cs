using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Gauges
{
    /// <summary>
    /// Represents a thermometer gauge (round-face).
    /// </summary>
    public class ThermoGauge : RoundFaceGaugeBase
    {
        public ThermoGauge(string id, GaugeConfig config)
            : base(id, config.Unit, config)
        {
        }
    }
}