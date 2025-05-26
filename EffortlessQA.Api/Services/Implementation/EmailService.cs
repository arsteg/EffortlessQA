using EffortlessQA.Api.Services.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace EffortlessQA.Api.Services.Implementation
{
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body)
        {
            throw new NotImplementedException();
        }
    }
}
