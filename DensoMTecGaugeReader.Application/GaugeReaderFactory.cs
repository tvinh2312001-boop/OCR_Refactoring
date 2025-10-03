using DensoMTecGaugeReader.Core.Common;
using DensoMTecGaugeReader.Core.Common.Enums;
using DensoMTecGaugeReader.Core.Common.Errors;
using DensoMTecGaugeReader.Core.Contracts;
using DensoMTecGaugeReader.Core.Contracts.Gauges;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Core.Gauges;
using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Infrastructure.Detectors;
using System;

namespace DensoMTecGaugeReader.Application
{
    public static class GaugeReaderFactory
    {
        public static (IGaugeFaceDetector? faceDetector,
                       IGaugeHandDetector? handDetector) Create(string id, GaugeConfig config)
        {
            IGaugeFaceDetector? faceDetector = null;
            IGaugeHandDetector? handDetector = null;

            // chọn detector theo FaceType
            switch (config.FaceType)
            {
                case GaugeFaceType.Round:
                    faceDetector = new RoundFaceDetector();
                    handDetector = new GaugeHandDetector();
                    break;

                case GaugeFaceType.Square:
                    faceDetector = new SquareFaceDetector();
                    handDetector = new GaugeHandDetector();
                    break;

                case GaugeFaceType.Digital:
                    faceDetector = null;
                    handDetector = null;
                    break;
                default:
                    var result = Result.Fail(ResultMessages.UnsupportGagueFance);
                    throw new GaugeReadingProcessException(GaugeErrorCode.UnsupportGaugeFace);
            }

            IGauge gauge = config.GaugeType switch
            {
                GaugeType.Pressure => new PressureGauge(id, config),
                GaugeType.Thermo => new ThermoGauge(id, config),
                _ => throw new ArgumentOutOfRangeException(nameof(config.GaugeType), "Unsupported gauge type")
            };

            return (faceDetector, handDetector);
        }
    }
}