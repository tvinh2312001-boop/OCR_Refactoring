using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;

namespace DensoMTecGaugeReader.Core.Gauges
{
    /// <summary>
    /// Base class for square-face gauges.
    /// Currently a placeholder for future extension.
    /// </summary>
    public abstract class SquareGaugeBase : IGauge
    {
        public string Id { get; protected set; }
        public string Unit { get; protected set; }
        public GaugeConfig Config { get; protected set; }

        protected SquareGaugeBase(string id, string unit, GaugeConfig config)
        {
            Id = id;
            Unit = unit;
            Config = config;
        }

        /// <summary>
        /// To be implemented in future when square gauge detection is supported.
        /// </summary>
        public abstract MeasurementResult ReadValue();
    }
}