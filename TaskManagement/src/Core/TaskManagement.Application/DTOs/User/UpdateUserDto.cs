using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.User
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Department { get; set; }
    }
}
