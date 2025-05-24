using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        // Optional: ISO Code (e.g., IN for India, US for United States)
        [StringLength(10)]
        public string Code { get; set; }
    }
}
