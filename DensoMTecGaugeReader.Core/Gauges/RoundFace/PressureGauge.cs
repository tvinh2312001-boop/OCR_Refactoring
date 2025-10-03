using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Gauges
{
    /// <summary>
    /// Represents a pressure gauge (round-face).
    /// </summary>
    public class PressureGauge : RoundFaceGaugeBase
    {
        public PressureGauge(string id, GaugeConfig config)
            : base(id, config.Unit, config)
        {
        }
    }
}
