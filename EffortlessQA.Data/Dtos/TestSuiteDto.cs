using System.ComponentModel.DataAnnotations;

namespace EffortlessQA.Data.Dtos
{
    public class TestSuiteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
		public bool IsEditing { get; set; }
		public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public Guid? ParentSuiteId { get; set; }
    }

    public class CreateTestSuiteDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentSuiteId { get; set; }
        public Guid ProjectId { get; set; }
    }

    public class UpdateTestSuiteDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public Guid? ParentSuiteId { get; set; }
    }

    public class TestSuiteCreateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public Guid ProjectId { get; set; }

        [Required, MaxLength(50)]
        public string TenantId { get; set; }
    }

    public class TestSuiteUpdateDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
