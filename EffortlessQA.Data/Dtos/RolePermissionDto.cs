using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class RolePermissionDto
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RolePermissionCreateDto
    {
        [Required]
        public Guid RoleId { get; set; }

        [Required]
        public Guid PermissionId { get; set; }
    }
}
