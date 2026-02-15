using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Results
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string>? Errors { get; set; }

        protected Result(bool isSuccess, string message, List<string>? errors = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            Errors = errors;
        }

        public static Result Success(string message = "Operation completed successfully")
        {
            return new Result(true, message);
        }

        public static Result Failure(string message, List<string>? errors = null)
        {
            return new Result(false, message, errors);
        }

        public static Result Failure(List<string> errors)
        {
            return new Result(false, "Operation failed", errors);
        }
    }

    public class Result<T> : Result
    {
        public T? Data { get; set; }

        private Result(bool isSuccess, string message, T? data = default, List<string>? errors = null)
            : base(isSuccess, message, errors)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string message = "Operation completed successfully")
        {
            return new Result<T>(true, message, data);
        }

        public static new Result<T> Failure(string message, List<string>? errors = null)
        {
            return new Result<T>(false, message, default, errors);
        }

        public static new Result<T> Failure(List<string> errors)
        {
            return new Result<T>(false, "Operation failed", default, errors);
        }
    }
}
