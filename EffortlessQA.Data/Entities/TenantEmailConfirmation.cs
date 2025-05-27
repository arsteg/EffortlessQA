using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    public class TenantEmailConfirmation : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string TenantId { get; set; }

        [Required]
        public string Token { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
