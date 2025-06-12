using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class InviteModel
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public RoleType RoleType { get; set; }
        public Guid ProjectId { get; set; }
    }
}
