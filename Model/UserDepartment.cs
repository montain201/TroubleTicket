using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Model
{
    [Keyless]
    public class UserDepartment
    {
        public string UserId { get; set; }
        public int DepartmentId { get; set; }
    }
}
