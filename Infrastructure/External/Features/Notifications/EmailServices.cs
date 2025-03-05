using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Shared.Abstractions;
using Domain.Features.Users.Entities;
using Domain.Features.Users.Events;

namespace Infrastructure.External.Features.Notifications;

public class EmailServices : IEmailService
{
    public Task SendConfirmationEmail(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        if (user.VerificationCode == null)
            throw new ArgumentNullException(nameof(user.VerificationCode));

        // Send email

        var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential("your-email@gmail.com", "your-password"),
            EnableSsl = true,
        };

        var mailMessage = new System.Net.Mail.MailMessage
        {
            From = new System.Net.Mail.MailAddress("your-email@gmail.com"),
            Subject = "Email Verification",
            Body = $"Please verify your email using this code: {user.VerificationCode}",
            IsBodyHtml = false
        };
        mailMessage.To.Add(user.Email);

        await smtpClient.SendMailAsync(mailMessage);
        return Task.CompletedTask;

    }
}
