using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Services.Measurement;

namespace DensoMTecGaugeReader.Core.Gauges
{
    /// <summary>
    /// Base class for round gauges.
    /// Provides shared measurement logic using round face and config.
    /// </summary>
    public abstract class RoundGaugeBase : IGauge
    {
        public abstract string Name { get; }

        protected readonly GaugeConfig _config;

        protected RoundGaugeBase(GaugeConfig config)
        {
            _config = config;
        }

        public virtual MeasurementResult Measure(GaugeFaceInfo face, GaugeHandInfo? hand)
        {
            if (hand == null)
                throw new GaugeException(GaugeErrorCode.HandNotDetected, $"No hand detected for {Name}.");

            double scaleAngle = AngleCalculator.Normalize(hand.Angle, face, hand);

            double value = ScaleMapper.Map(scaleAngle,
                                           _config.MinValue,
                                           _config.MaxValue,
                                           _config.StartAngle,
                                           _config.EndAngle);

            return new MeasurementResult(hand.Angle, scaleAngle, value, _config.Unit);
        }
    }
}
