using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("Validation failed")
        {
            Errors = errors;
        }

        public Dictionary<string, string[]>? Errors { get; }
    }
}
