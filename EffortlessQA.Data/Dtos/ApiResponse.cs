using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EffortlessQA.Data.Entities;

namespace EffortlessQA.Data.Dtos
{
    public class ApiResponse<T>
    {
        public T Data { get; set; }
        public object Meta { get; set; }
        public ErrorResponse Error { get; set; }
    }

    public class ErrorResponse
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class RegisterDto
    {
        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(255, MinimumLength = 8)]
        public string Password { get; set; }

        [Required, StringLength(50)]
        public string TenantId { get; set; }
    }

    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
