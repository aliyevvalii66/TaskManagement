using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Application.DTOs.Common
{
    public class PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
