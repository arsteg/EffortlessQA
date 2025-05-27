using System.Net;
using System.Text.RegularExpressions;
using EffortlessQA.Api.Services.Interface;
using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace EffortlessQA.Api.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _senderEmail;
        private readonly string _senderName;
        private readonly string _apiKey;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _senderEmail =
                _configuration["SendGrid:SenderEmail"]
                ?? throw new ArgumentNullException("SendGrid:SenderEmail is not configured.");
            _senderName = _configuration["SendGrid:SenderName"] ?? "EffortlessQA Team";
            _apiKey =
                _configuration["SendGrid:ApiKey"]
                ?? throw new ArgumentNullException("SendGrid:ApiKey is not configured.");
        }

        public async Task SendEmailAsync(
            string toEmail,
            string subject,
            string htmlContent,
            string? plainTextContent = null
        )
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("Recipient email cannot be empty.", nameof(toEmail));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(htmlContent))
                throw new ArgumentException("HTML content cannot be empty.", nameof(htmlContent));

            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent ?? StripHtml(htmlContent), // Fallback to stripped HTML if plain text not provided
                htmlContent
            );

            var response = await client.SendEmailAsync(msg);

            if (
                response.StatusCode != HttpStatusCode.OK
                && response.StatusCode != HttpStatusCode.Accepted
            )
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                throw new Exception($"Failed to send email: {response.StatusCode} - {errorBody}");
            }
        }

        public async Task SendRegistrationConfirmationAsync(
            string toEmail,
            string userName,
            string confirmationLink
        )
        {
            var subject = "Welcome to EffortlessQA - Confirm Your Email";
            var htmlContent = GenerateRegistrationEmailHtml(userName, confirmationLink);
            var plainTextContent = GenerateRegistrationEmailPlainText(userName, confirmationLink);

            await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
        }

        public async Task SendTenantConfirmationAsync(
            string toEmail,
            string tenantName,
            string confirmationLink
        )
        {
            var subject = "EffortlessQA - Confirm Your Company Email";
            var htmlContent = GenerateTenantEmailHtml(tenantName, confirmationLink);
            var plainTextContent = GenerateTenantEmailPlainText(tenantName, confirmationLink);
            await SendEmailAsync(toEmail, subject, htmlContent, plainTextContent);
        }

        private string GenerateRegistrationEmailHtml(string userName, string confirmationLink)
        {
            return $@"
                <html>
                <body style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Welcome to EffortlessQA, {userName}!</h2>
                    <p>Thank you for registering with EffortlessQA. To complete your registration, please confirm your email address by clicking the button below:</p>
                    <p style='margin: 20px 0;'>
                        <a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Email</a>
                    </p>
                    <p>If the button doesn't work, copy and paste this link into your browser:</p>
                    <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                    <p>If you didn't sign up for EffortlessQA, please ignore this email.</p>
                    <p>Best regards,<br/>The EffortlessQA Team</p>
                </body>
                </html>";
        }

        private string GenerateRegistrationEmailPlainText(string userName, string confirmationLink)
        {
            return $@"Welcome to EffortlessQA, {userName}!

Thank you for registering with EffortlessQA. To complete your registration, please confirm your email address by visiting the following link:

{confirmationLink}

If you didn't sign up for EffortlessQA, please ignore this email.

Best regards,
The EffortlessQA Team";
        }

        private string StripHtml(string html)
        {
            // Simple HTML stripping for plain text fallback
            return Regex.Replace(html, "<[^>]+>", string.Empty).Replace("&nbsp;", " ").Trim();
        }

        private string GenerateTenantEmailHtml(string tenantName, string confirmationLink)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; color: #333;'>
                <h2>Welcome to EffortlessQA, {tenantName}!</h2>
                <p>Thank you for creating a company account with EffortlessQA. To complete your company registration, please confirm your company email address by clicking the button below:</p>
                <p style='margin: 20px 0;'>
                    <a href='{confirmationLink}' style='background-color: #007bff; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>Confirm Company Email</a>
                </p>
                <p>If the button doesn't work, copy and paste this link into your browser:</p>
                <p><a href='{confirmationLink}'>{confirmationLink}</a></p>
                <p>If you didn't create this company account, please ignore this email.</p>
                <p>Best regards,<br/>The EffortlessQA Team</p>
            </body>
            </html>";
        }

        private string GenerateTenantEmailPlainText(string tenantName, string confirmationLink)
        {
            return $@"Welcome to EffortlessQA, {tenantName}!

Thank you for creating a company account with EffortlessQA. To complete your company registration, please confirm your company email address by visiting:

{confirmationLink}

If you didn't create this company account, please ignore this email.

Best regards,
The EffortlessQA Team";
        }
    }
}
