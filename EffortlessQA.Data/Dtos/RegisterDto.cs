using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress, MaxLength(255)]
        public string Email { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }
}
