using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class RoleCreateDto
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required, MaxLength(20)]
        [RegularExpression("Admin|Tester")]
        public string RoleType { get; set; }
    }
}
