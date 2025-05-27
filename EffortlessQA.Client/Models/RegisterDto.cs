namespace EffortlessQA.Client.Models
{
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string TenantName { get; set; } = string.Empty;
        public string TenantAddress { get; set; } = string.Empty;
    }
}
