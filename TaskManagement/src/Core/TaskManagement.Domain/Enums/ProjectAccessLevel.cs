using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManagement.Domain.Enums
{
    public enum ProjectAccessLevel
    {
        Owner = 0,
        Admin = 1,
        Member = 2,
        Viewer = 3
    }
}
