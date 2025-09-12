using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Gauges
{
    public class PressureGauge : RoundGaugeBase
    {
        public override string Name => "Pressure Gauge";

        public PressureGauge(GaugeConfig config) : base(config) { }
    }
}