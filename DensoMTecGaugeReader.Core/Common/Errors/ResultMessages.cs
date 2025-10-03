using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoMTecGaugeReader.Core.Common.Errors
{
    public static class ResultMessages
    {
        // Success
        public const string AngleMapped = "Angle mapped successfully.";
        public const string OperationCompleted = "Operation completed successfully.";

        // Errors
        public const string EmptyTickMap = "NumberToAngleMap is empty.";
        public const string OutOfRange = "Angle is out of range.";
        public const string InvalidInput = "Invalid input provided.";
        public const string EmptyImage = "Image not found or is empty.";
        public const string GaugeFaceNotFound = "Cannot find gauge's face.";
        public const string GaugeHandNotFound = "Cannot find gauge's base line.";
        public const string ScreenNotFound = "Cannot find Toshiba Carrier screen.";
        public const string ValueOutOfRange = "Value is out of range.";
        public const string UnsupportGagueFance = "Digital gauges are not supported yet.";

        // Warnings
        public const string ClampedMin = "Angle clamped to minimum value.";
        public const string ClampedMax = "Angle clamped to maximum value.";
    }
}
