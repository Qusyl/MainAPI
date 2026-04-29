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
    public class Result<TError>
    {
        public TError Error { get; set; }

        public bool IsSuccess { get; set; }

        private Result(TError error, bool isSuccess)
        {

            Error = error;
            IsSuccess = isSuccess;
        }
        public static Result<TError> Success => new Result<TError>(default, true);
        public static Result<TError> Failure(TError error) => new Result<TError>(error, false);
    }
}
