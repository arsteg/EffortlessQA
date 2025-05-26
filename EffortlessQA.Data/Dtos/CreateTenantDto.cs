using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class CreateTenantDto
    {
        public string Name { get; set; } // Organization name

        public string ContactPerson { get; set; }

        public string Email { get; set; }

        public long? Phone { get; set; } // Use long to handle all number lengths

        public string? Description { get; set; }
    }
}
