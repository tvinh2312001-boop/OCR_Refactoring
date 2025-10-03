using DensoMTecGaugeReader.Core.Common.Errors;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Contracts.Gauges;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Core.Services.Measurement;

namespace DensoMTecGaugeReader.Application.Services
{
    /// <summary>
    /// Orchestrator cho analog gauges: Face → Hand → Angle → Value.
    /// </summary>
    public class GaugeMeasurementService
    {
        private readonly IGaugeFaceDetector _faceDetector;
        private readonly IGaugeHandDetector _handDetector;
        private readonly AngleCalculator _angleCalculator;
        private readonly ScaleMapper _scaleMapper;

        public GaugeMeasurementService(
            IGaugeFaceDetector faceDetector,
            IGaugeHandDetector handDetector)
        {
            _faceDetector = faceDetector;
            _handDetector = handDetector;
            _angleCalculator = new AngleCalculator();
            _scaleMapper = new ScaleMapper();
        }

        public MeasurementResult ReadGauge(string imagePath, GaugeConfig config)
        {
            // 1. Detect face
            var face = _faceDetector.DetectFace(imagePath);
            if (face == null)
                throw new GaugeReadingProcessException(GaugeErrorCode.GaugeFaceNotFound);

            // 2. Detect hand
            var hand = _handDetector.DetectHand(imagePath, face);
            if (hand == null)
                throw new GaugeReadingProcessException(GaugeErrorCode.GaugeFaceNotFound);

            // 3. Angle
            var angle = _angleCalculator.CalculateAngle(face, hand);

            // 4. Map value
            var value = _scaleMapper.MapAngleToValue(config, angle);

            return new MeasurementResult
            {
                RawAngle = angle,
                Value = value,
                Unit = config.Unit
            };
        }
    }
}
