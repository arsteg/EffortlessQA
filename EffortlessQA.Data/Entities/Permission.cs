using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Permission : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        // [Index(IsUnique = true)]
        public string Name { get; set; } // e.g., "CreateTestCase", "DeleteProject"

        public string? Description { get; set; }

        // Navigation property
        public List<RolePermission> RolePermissions { get; set; } = new();
    }
}
