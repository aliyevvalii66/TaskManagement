using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Exceptions
{
    public class UnauthorizedException : ApplicationException
    {
        public UnauthorizedException(string message = "Unauthorized access")
            : base(message)
        {
        }
    }
}
