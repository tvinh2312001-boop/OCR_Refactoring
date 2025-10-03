using DensoMTecGaugeReader.Core.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DensoMTecGaugeReader.Core.Common
{
    public class Result
    {
        public ResultStatus Status { get; }
        public string Message { get; }

        public bool Success => Status == ResultStatus.Success;
        public bool Failure => Status == ResultStatus.Failure;
        public bool HasWarning => Status == ResultStatus.Warning;

        private Result(ResultStatus status, string message)
        {
            Status = status;
            Message = message;
        }

        public static Result Ok(string message = null)
            => new Result(ResultStatus.Success, message);

        public static Result Fail(string message)
            => new Result(ResultStatus.Failure, message);

        public static Result Warn(string message)
            => new Result(ResultStatus.Warning, message);
    }
}
