using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Result<TValue, TError>
    {
        public TValue Value { get; set; }   

        public TError Error { get; set; }

        public bool IsSuccess { get; set; }

        private Result(TValue value, TError error, bool isSuccess )
        {
            Value = value;
            Error = error;
            IsSuccess = isSuccess;
        }
        public static Result<TValue, TError> Success(TValue value) => new Result<TValue, TError>(value, default, true);
        public static Result<TValue, TError> Failure (TError error) => new Result<TValue, TError>(default, error, false);
    }
}
