using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class TestFolderDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid ProjectId { get; set; }
        public string TenantId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTestFolderDto
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateTestFolderDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
