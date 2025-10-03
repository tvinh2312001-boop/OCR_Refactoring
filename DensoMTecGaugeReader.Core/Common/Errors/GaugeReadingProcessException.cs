using DensoMTecGaugeReader.Core.Common.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DensoMTecGaugeReader.Core.Exceptions
{
    /// <summary>
    /// Custom exception for handling gauge reading errors.
    /// Wraps a GaugeErrorCode with a human-readable message.
    /// </summary>
    public class GaugeReadingProcessException : Exception
    {
        public GaugeErrorCode ErrorCode { get; }

        public GaugeReadingProcessException(GaugeErrorCode errorCode)
            : base(GetErrorMessage(errorCode))
        {
            ErrorCode = errorCode;
        }

        private static string GetErrorMessage(GaugeErrorCode errorCode)
        {
            return errorCode switch
            {
                GaugeErrorCode.EmptyImage => "Image not found or is empty.",
                GaugeErrorCode.GaugeFaceNotFound => "Cannot find gauge's face.",
                GaugeErrorCode.GaugeHandNotFound => "Cannot find gauge's hand.",
                GaugeErrorCode.GaugeBaseLineNotFound => "Cannot find gauge's base line.",
                GaugeErrorCode.ScreenNotFound => "Cannot find Toshiba Carrier screen.",
                GaugeErrorCode.ValueOutOfRange => "Value is out of range.",
                GaugeErrorCode.UnsupportGaugeFace => "Digital gauges are not supported yet.",
                _ => "An unknown error occurred."
            };
        }
    }
}


