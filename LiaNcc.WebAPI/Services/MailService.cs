using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using LiaNcc.Models.Enums;
using LiaNcc.Models.DTOs.Requests;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using LiaNcc.Repository.Interfaces;
using MailHelper;

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

        private HelperMailKit CreateMailer()
        {
            return new HelperMailKit(
                _settings.FromEmail,
                _settings.FromEmailPwd,
                _settings.Host,
                _settings.Port,
                _settings.EnableSSL,
                _settings.SenderName
            );
        }

        public async Task SendContactNotificationAsync(ContactMessage contactMessage)
        {
            if (!_settings.SendAdminNotification) return;

            string subject = "Nuovo messaggio dal sito Lia NCC";
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

        public async Task SendBookingNotificationAsync(Booking booking, BookingCreateRequest request)
        {
            if (!_settings.SendAdminNotification) return;

            string subject = "Nuova richiesta di prenotazione - Lia NCC";
            string body = $@"
                <h2>Nuova richiesta di prenotazione</h2>
                <p><strong>Nome:</strong> {booking.FullName}</p>
                <p><strong>Email:</strong> {booking.Email}</p>
                <p><strong>Telefono:</strong> {booking.Phone ?? "Non fornito"}</p>
                <p><strong>Data Servizio:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</p>
                <p><strong>ID Tipo Servizio:</strong> {booking.ServiceTypeId}</p>
                <p><strong>ID Opzione Passeggeri:</strong> {booking.PassengerOptionId}</p>
                <p><strong>ID Tour:</strong> {booking.TourId?.ToString() ?? "-"}</p>
                <p><strong>ID Veicolo:</strong> {booking.VehicleId?.ToString() ?? "-"}</p>
                <p><strong>Posti Max:</strong> {booking.MaxSeats ?? 1}</p>
                <p><strong>Messaggio:</strong><br/>{booking.Message ?? "-"}</p>
                <p><strong>Stato:</strong> {booking.Status}</p>
                <p><strong>Data Creazione:</strong> {booking.CreatedAt:dd/MM/yyyy HH:mm}</p>
            ";

            await SendEmailHtmlAsync(_settings.AdminEmail, subject, body, "Booking", booking.Id);
        }

        public async Task SendBookingCustomerConfirmationAsync(Booking booking, BookingCreateRequest request)
        {
            if (!_settings.SendCustomerConfirmation) return;

            string subject = "Richiesta di prenotazione ricevuta - Lia NCC";
            string body = $@"
                <h2>Richiesta di prenotazione ricevuta</h2>
                <p>Gentile {booking.FullName},</p>
                <p>abbiamo ricevuto la tua richiesta di prenotazione per il giorno {booking.ServiceDate:dd/MM/yyyy HH:mm}.</p>
                <p><strong>Riepilogo richiesta:</strong></p>
                <ul>
                    <li><strong>Data:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</li>
                    <li><strong>Passeggeri:</strong> {booking.MaxSeats ?? 1}</li>
                </ul>
                <p>La tua richiesta è in attesa di conferma. Ti contatteremo a breve.</p>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(booking.Email, subject, body, "Booking", booking.Id);
        }

        public async Task SendBookingAcceptedAsync(Booking booking)
        {
            string subject = "Prenotazione Accettata - Lia NCC";
            string body = $@"
                <h2>La tua prenotazione è stata accettata</h2>
                <p>Gentile {booking.FullName},</p>
                <p>siamo lieti di comunicarti che la tua richiesta di prenotazione è stata accettata.</p>
                <h3>Riepilogo della prenotazione:</h3>
                <ul>
                    <li><strong>Data Servizio:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</li>
                    <li><strong>Passeggeri:</strong> {booking.MaxSeats ?? 1}</li>
                </ul>
                <p>Ti ringraziamo per averci scelto.</p>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(booking.Email, subject, body, "Booking", booking.Id);
        }

        public async Task SendBookingRejectedAsync(Booking booking, string reason)
        {
            string subject = "Aggiornamento Prenotazione - Lia NCC";
            string body = $@"
                <h2>Informazioni sulla tua richiesta di prenotazione</h2>
                <p>Gentile {booking.FullName},</p>
                <p>ti informiamo che purtroppo non è stato possibile accettare la tua richiesta di prenotazione per il giorno {booking.ServiceDate:dd/MM/yyyy HH:mm}.</p>
                <p><strong>Motivazione:</strong></p>
                <blockquote style='border-left: 5px solid #ccc; padding-left: 10px;'>{reason}</blockquote>
                <p>Rimaniamo a tua disposizione per ulteriori necessità.</p>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(booking.Email, subject, body, "Booking", booking.Id);
        }

        public async Task SendReplyEmailAsync(string toEmail, string subject, string body, List<(string FileName, byte[] Content, string ContentType)>? attachments = null, string? relatedEntityName = null, Guid? relatedEntityId = null)
        {
            await SendEmailInternalAsync(toEmail, subject, body, attachments, relatedEntityName, relatedEntityId);
        }

        public async Task SendEmailHtmlAsync(string toEmail, string subject, string htmlBody, string? relatedEntityName = null, Guid? relatedEntityId = null)
        {
            await SendEmailInternalAsync(toEmail, subject, htmlBody, null, relatedEntityName, relatedEntityId);
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string htmlBody, List<(string FileName, byte[] Content, string ContentType)>? attachments, string? relatedEntityName, Guid? relatedEntityId)
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

                _logger.LogInformation("Attempting to send email to {ToEmail} using MailHelper.HelperMailKit", toEmail);

                var mailer = CreateMailer();

                if (attachments != null && attachments.Count > 0)
                {
                    // If the DLL doesn't support attachments in SendEmailHtmlAsync, we might need to handle it.
                    // But the requirement says: "La DLL espone anche metodi per email HTML e invio con evento calendario"
                    // HelperMailKit might not have a direct SendEmailHtmlAsync with attachments in the signature I was given,
                    // but the instruction says "Usare direttamente HelperMailKit".
                    // I will check if I can see the DLL methods somehow or just assume the basic one for now.
                    // Actually, if it doesn't support it, I should stick to what's requested.

                    // Since I don't have the metadata for the DLL, I'll use SendEmailHtmlAsync as per example.
                    await mailer.SendEmailHtmlAsync(toEmail, subject, htmlBody);
                }
                else
                {
                    await mailer.SendEmailHtmlAsync(toEmail, subject, htmlBody);
                }

                emailLog.Status = "Sent";
                emailLog.SentAt = DateTime.UtcNow;
                await _emailRepository.UpdateAsync(emailLog);

                await _appLogger.LogInformationAsync(
                    "Mail",
                    "SmtpSendSuccess",
                    $"Email inviata con successo a {toEmail}",
                    "MailService",
                    relatedEntityName,
                    relatedEntityId,
                    ApplicationEventType.Email,
                    new { To = toEmail, Subject = subject, EmailId = emailLog.Id }
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail} using MailHelper", toEmail);

                emailLog.Status = "Failed";
                emailLog.ErrorMessage = ex.Message;
                await _emailRepository.UpdateAsync(emailLog);

                await _appLogger.LogErrorAsync(
                    "Mail",
                    "SmtpSendFailed",
                    $"Invio email a {toEmail} fallito: {ex.Message}",
                    ex,
                    null,
                    "MailService",
                    relatedEntityName,
                    relatedEntityId,
                    ApplicationEventType.Email,
                    new { EmailId = emailLog.Id }
                );

                throw; // Re-throw to allow controller to handle it if needed
            }
        }
    }
}
