using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Role : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("Admin|Tester", ErrorMessage = "Role must be Admin or Tester")]
        public RoleType RoleType { get; set; } // "Admin" or "Tester"
        public List<RolePermission> RolePermissions { get; set; } = new();
        public string TenantId { get; set; }
    }
}
