using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoMTecGaugeReader.Core.Common.Errors
{
    /// <summary>
    /// Defines error codes for gauge reading processes.
    /// </summary>
    public enum GaugeErrorCode
    {
        EmptyImage,
        GaugeFaceNotFound,
        GaugeHandNotFound,
        GaugeBaseLineNotFound,
        ScreenNotFound,
        ValueOutOfRange,
        UnsupportGaugeFace
    }
}
