using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EffortlessQA.Data.Entities
{
    [Auditable]
    public class Address : EntityBase
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(50)]
        public string Address_Line1 { get; set; }
        public string Address_Line2 { get; set; }
        public string Address_Line3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int? CountryId { get; set; }

        [ForeignKey("CountryId")]
        public Country Country { get; set; }
        public string Pincode { get; set; }

        [MaxLength(255)]
        public string? BillingContactEmail { get; set; }
        public string TenantId { get; set; }
    }
}
