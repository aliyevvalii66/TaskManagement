using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Shared.Exceptions
{
    public class NotFoundException : ApplicationException
    {
        public NotFoundException(string resourceName, object resourceId)
            : base($"{resourceName} with id {resourceId} not found.")
        {
        }

        public NotFoundException(string message) : base(message)
        {
        }
    }
}
