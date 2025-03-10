using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.External.Features.Notifications;

public class EmailServices : IEmailService
{
    ILogger<EmailServices> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private string SMTP { get; set; } = null!;
    private string EMAIL { get; set; } = null!;
    private string PASSWORD { get; set; } = null!;

    public EmailServices(
        ILogger<EmailServices> logger,
        IConfiguration configuration,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        SMTP = configuration.GetSection("Email:Host").Value!;
        EMAIL = configuration.GetSection("Email:UserName").Value!;
        PASSWORD = configuration.GetSection("Email:Password").Value!;
    }

    public async Task SendConfirmationEmail(UserCreatedDomainEvent user)
    {
        try
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (string.IsNullOrEmpty(user.VerificationCode))
                throw new ArgumentNullException(nameof(user.VerificationCode));

            // Send email

            var smtpClient = new SmtpClient(SMTP, 587)
            {
                Credentials = new NetworkCredential(EMAIL, PASSWORD, SMTP),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(EMAIL),
                Sender = new MailAddress(EMAIL),
                Subject = "Email Verification",
                Body = $"Please verify your email using this code: {user.VerificationCode}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(user.Email!);

            await smtpClient.SendMailAsync(mailMessage);
            smtpClient.Dispose();
        }
        catch (SmtpException ex)
        {
            _logger.LogError(ex, "Failed to send email. SMTP server response: {Message}", ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while sending email.");
            throw;
        }
    }
}
