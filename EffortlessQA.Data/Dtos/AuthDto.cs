using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class ChangePasswordDto
    {
        public Guid UserId { get; set; } // Add this
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
