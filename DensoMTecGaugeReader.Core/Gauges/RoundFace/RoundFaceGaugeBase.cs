using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Services.Measurement;

namespace DensoMTecGaugeReader.Core.Gauges
{
    /// <summary>
    /// Base class for round-face gauges (analog).
    /// Provides logic for converting hand position to measurement value.
    /// </summary>
    public abstract class RoundFaceGaugeBase : IGauge
    {
        public string Id { get; protected set; }
        public string Unit { get; protected set; }
        public GaugeConfig Config { get; protected set; }
        public GaugeFaceInfo Face { get; protected set; }
        public GaugeHandInfo Hand { get; protected set; }

        protected RoundFaceGaugeBase(string id, string unit, GaugeConfig config)
        {
            Id = id;
            Unit = unit;
            Config = config;
        }

        /// <summary>
        /// Sets the detected face info (from detector).
        /// </summary>
        public void SetFace(GaugeFaceInfo faceInfo)
        {
            Face = faceInfo;
        }

        /// <summary>
        /// Sets the detected hand info (from detector).
        /// </summary>
        public void SetHand(GaugeHandInfo handInfo)
        {
            Hand = handInfo;
        }

        /// <summary>
        /// Reads the current gauge value by calculating angle and mapping to real value.
        /// </summary>
        public MeasurementResult ReadValue()
        {
            var angleCalculator = new AngleCalculator();
            var scaleMapper = new ScaleMapper();

            double angle = angleCalculator.CalculateAngle(Face, Hand);
            double value = scaleMapper.MapAngleToValue(Config, angle);

            return new MeasurementResult
            {
                RawAngle = angle,
                Value = value,
                Unit = Unit
            };
        }
    }
}