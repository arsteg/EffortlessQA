using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Dtos
{
    public class PdfGenerationDto
    {
        public string Title { get; set; } = "Report";
        public string FileName { get; set; } = "report.pdf";
        public int FontSizeTitle { get; set; } = 16;
        public int FontSizeHeader { get; set; } = 12;
        public int FontSizeBody { get; set; } = 10;
        public List<PdfColumnDto> Columns { get; set; } = new List<PdfColumnDto>();
        public List<Dictionary<string, string>> Data { get; set; } =
            new List<Dictionary<string, string>>();
    }

    public class PdfColumnDto
    {
        public string Field { get; set; } = string.Empty;
        public string Header { get; set; } = string.Empty;
    }
}
