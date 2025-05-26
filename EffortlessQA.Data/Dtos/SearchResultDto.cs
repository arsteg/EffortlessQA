using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class SearchResultDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string EntityType { get; set; } // e.g., "TestCase", "TestSuite", "TestRun", "Requirement", "Defect"
        public Guid ProjectId { get; set; }
        public string[]? Tags { get; set; }
    }
}
