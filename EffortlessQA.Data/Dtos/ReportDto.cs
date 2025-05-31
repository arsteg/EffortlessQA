using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; }
    }
}
