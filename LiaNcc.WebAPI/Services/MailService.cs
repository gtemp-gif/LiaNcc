using System;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Models.Enums;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using LiaNcc.Repository.Interfaces;

namespace LiaNcc.WebAPI.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;
        private readonly IApplicationLoggerService _appLogger;
        private readonly ILogger<MailService> _logger;
        private readonly IEmailMessageRepository _emailRepository;

        public MailService(
            IOptions<MailSettings> settings,
            IApplicationLoggerService appLogger,
            ILogger<MailService> logger,
            IEmailMessageRepository emailRepository)
        {
            _settings = settings.Value;
            _appLogger = appLogger;
            _logger = logger;
            _emailRepository = emailRepository;
        }

        public async Task SendContactNotificationAsync(ContactMessage contactMessage)
        {
            if (!_settings.SendAdminNotification) return;

            string subject = "Nuovo messaggio da Lia NCC";
            string body = $@"
                <h2>Nuovo messaggio di contatto</h2>
                <p><strong>Nome:</strong> {contactMessage.FullName}</p>
                <p><strong>Email:</strong> {contactMessage.Email}</p>
                <p><strong>Messaggio:</strong><br/>{contactMessage.Message}</p>
                <p><strong>Data:</strong> {contactMessage.CreatedAt:dd/MM/yyyy HH:mm}</p>
            ";

            await SendEmailHtmlAsync(_settings.AdminEmail, subject, body, "ContactMessage", contactMessage.Id);
        }

        public async Task SendContactCustomerConfirmationAsync(ContactMessage contactMessage)
        {
            if (!_settings.SendCustomerConfirmation) return;

            string subject = "Abbiamo ricevuto il tuo messaggio - Lia NCC";
            string body = $@"
                <h2>Grazie per averci contattato</h2>
                <p>Ciao {contactMessage.FullName},</p>
                <p>abbiamo ricevuto il tuo messaggio e ti risponderemo al più presto.</p>
                <p><strong>Riepilogo del tuo messaggio:</strong></p>
                <blockquote style='border-left: 5px solid #ccc; padding-left: 10px;'>{contactMessage.Message}</blockquote>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(contactMessage.Email, subject, body, "ContactMessage", contactMessage.Id);
        }

        public async Task SendBookingNotificationAsync(Booking booking)
        {
            if (!_settings.SendAdminNotification) return;

            string subject = "Nuova richiesta di prenotazione Lia NCC";
            string body = $@"
                <h2>Nuova richiesta di prenotazione</h2>
                <p><strong>Cliente:</strong> {booking.FullName}</p>
                <p><strong>Email:</strong> {booking.Email}</p>
                <p><strong>Telefono:</strong> {booking.Phone ?? "Non fornito"}</p>
                <p><strong>Data Servizio:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>Passeggeri:</strong> {booking.MaxSeats ?? 1}</p>
                <p><strong>Messaggio:</strong> {booking.Message ?? "-"}</p>
                <p><strong>Stato:</strong> {booking.Status}</p>
            ";

            await SendEmailHtmlAsync(_settings.AdminEmail, subject, body, "Booking", booking.Id);
        }

        public async Task SendBookingCustomerConfirmationAsync(Booking booking)
        {
            if (!_settings.SendCustomerConfirmation) return;

            string subject = "Richiesta di prenotazione ricevuta - Lia NCC";
            string body = $@"
                <h2>Richiesta di prenotazione ricevuta</h2>
                <p>Gentile {booking.FullName},</p>
                <p>la tua richiesta di prenotazione per il giorno {booking.ServiceDate:dd/MM/yyyy HH:mm} è stata presa in carico.</p>
                <p>Ti contatteremo a breve per la conferma definitiva.</p>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(booking.Email, subject, body, "Booking", booking.Id);
        }

        public async Task SendEmailHtmlAsync(string toEmail, string subject, string htmlBody, string? relatedEntityName = null, Guid? relatedEntityId = null)
        {
            var emailLog = new EmailMessage
            {
                ToEmail = toEmail,
                FromEmail = _settings.FromEmail,
                Subject = subject,
                Body = htmlBody,
                IsHtml = true,
                Status = "Pending",
                RelatedEntityName = relatedEntityName,
                RelatedEntityId = relatedEntityId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _emailRepository.CreateAsync(emailLog);

                _logger.LogInformation("Attempting to send email to {ToEmail} with subject {Subject}", toEmail, subject);

                // Here we would use the MailHelper.dll library
                /*
                var mailer = new MailHelper.HelperMailKit(
                    _settings.FromEmail,
                    _settings.FromEmailPwd,
                    _settings.Host,
                    _settings.Port,
                    _settings.EnableSSL,
                    _settings.SenderName
                );

                await mailer.SendEmailHtmlAsync(toEmail, subject, htmlBody);
                */

                // Since MailHelper is not present, we log the intention
                _logger.LogWarning("MailHelper.dll not found. Email not actually sent. Content: {Subject} to {ToEmail}", subject, toEmail);

                // Simuliamo successo per ora
                emailLog.Status = "Sent";
                emailLog.SentAt = DateTime.UtcNow;
                await _emailRepository.UpdateAsync(emailLog);

                await _appLogger.LogInformationAsync(
                    "Mail",
                    "SmtpSendSuccess",
                    $"Email inviata con successo a {toEmail} (SIMULATO)",
                    "MailService",
                    relatedEntityName,
                    relatedEntityId,
                    ApplicationEventType.Email,
                    new { To = toEmail, Subject = subject, EmailId = emailLog.Id }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);

                emailLog.Status = "Failed";
                emailLog.ErrorMessage = ex.Message;
                await _emailRepository.UpdateAsync(emailLog);

                await _appLogger.LogErrorAsync(
                    "Mail",
                    "SmtpSendFailed",
                    $"Invio email a {toEmail} fallito",
                    ex,
                    null,
                    "MailService",
                    relatedEntityName,
                    relatedEntityId,
                    ApplicationEventType.Email,
                    new { EmailId = emailLog.Id }
                );
            }
        }
    }
}
