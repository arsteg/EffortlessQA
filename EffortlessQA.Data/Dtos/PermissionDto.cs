using System.ComponentModel.DataAnnotations;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class PermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PermissionCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class PermissionUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class CreatePermissionDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdatePermissionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateRoleDto
    {
        public Guid UserId { get; set; }
        public RoleType RoleType { get; set; }
    }

    public class UpdateRoleDto
    {
        public Guid? UserId { get; set; }
        public RoleType? RoleType { get; set; }
    }

    public class AssignPermissionDto
    {
        public Guid PermissionId { get; set; }
    }
}
