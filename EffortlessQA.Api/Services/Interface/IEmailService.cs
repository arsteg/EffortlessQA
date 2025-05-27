namespace EffortlessQA.Api.Services.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlContent,
            string? plainTextContent = null
        );
        Task SendRegistrationConfirmationAsync(
            string toEmail,
            string userName,
            string confirmationLink
        );
        Task SendTenantConfirmationAsync(
            string toEmail,
            string tenantName,
            string confirmationLink
        );
    }
}
