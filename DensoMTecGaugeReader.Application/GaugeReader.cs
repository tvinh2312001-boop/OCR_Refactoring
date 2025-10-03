using DensoMTecGaugeReader.Core.Models;
using DensoMTecGaugeReader.Application.Services;
using DensoMTecGaugeReader.Core.Common.Enums;
using DensoMTecGaugeReader.Core.Exceptions;
using DensoMTecGaugeReader.Core.Common.Errors;
using DensoMTecGaugeReader.Core.Common;

namespace DensoMTecGaugeReader.Application
{
    public class GaugeReader
    {
        public MeasurementResult ReadGauge(string imagePath, GaugeConfig config, string id)
        {
            var (faceDetector, handDetector) = GaugeReaderFactory.Create(id, config);

            switch (config.FaceType)
            {
                case GaugeFaceType.Digital:
                    {
                        //var result = Result.Fail(ResultMessages.UnsupportGagueFance);
                        throw new GaugeReadingProcessException(GaugeErrorCode.UnsupportGaugeFace);
                    }

                case GaugeFaceType.Round:
                case GaugeFaceType.Square:
                    {
                        var service = new GaugeMeasurementService(faceDetector!, handDetector!);
                        return service.ReadGauge(imagePath, config);
                    }

                default:
                    var result = Result.Fail(ResultMessages.UnsupportGagueFance);
                    throw new GaugeReadingProcessException(GaugeErrorCode.GaugeFaceNotFound);
            }
        }


        //private string OcrStub(OpenCvSharp.Mat mat) => "123.4";
    }
}
