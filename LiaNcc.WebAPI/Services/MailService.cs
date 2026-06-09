using System;
using System.Collections.Generic;
using System.IO;
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
                <p><strong>Servizio:</strong> {booking.ServiceType?.Name ?? "-"}</p>
                <p><strong>Opzione Passeggeri:</strong> {booking.PassengerOption?.Name ?? "-"}</p>
                <p><strong>Tour:</strong> {booking.Tour?.Name ?? "-"}</p>
                <p><strong>Veicolo:</strong> {(booking.Vehicle != null ? $"{booking.Vehicle.Name} ({booking.Vehicle.VehicleCategory?.Name})" : "-")}</p>
                <p><strong>Posti Max:</strong> {booking.MaxSeats ?? 1}</p>
                <p><strong>Messaggio:</strong><br/>{booking.Message ?? "-"}</p>
                <p><strong>Stato:</strong> {booking.Status}</p>
                <p><strong>Provenienza:</strong> {booking.SourcePage ?? "Sito Web"}</p>
                <p><strong>Data Creazione:</strong> {booking.CreatedAt:dd/MM/yyyy HH:mm}</p>
            ";

            await SendEmailHtmlAsync(_settings.AdminEmail, subject, body, "Booking", booking.Id);
        }

        public async Task SendBookingCustomerConfirmationAsync(Booking booking, BookingCreateRequest request)
        {
            if (!_settings.SendCustomerConfirmation) return;

            string subject = "Richiesta di prenotazione ricevuta - Lia NCC";

            string requestedDetails = "";
            if (booking.Tour != null) requestedDetails += $"<li><strong>Tour:</strong> {booking.Tour.Name}</li>";
            if (booking.ServiceType != null) requestedDetails += $"<li><strong>Servizio:</strong> {booking.ServiceType.Name}</li>";
            if (booking.Vehicle != null) requestedDetails += $"<li><strong>Veicolo:</strong> {booking.Vehicle.Name} ({booking.Vehicle.VehicleCategory?.Name})</li>";
            if (booking.PassengerOption != null) requestedDetails += $"<li><strong>Opzione Passeggeri:</strong> {booking.PassengerOption.Name}</li>";

            string body = $@"
                <h2>Richiesta di prenotazione ricevuta</h2>
                <p>Gentile {booking.FullName},</p>
                <p>abbiamo ricevuto la tua richiesta di prenotazione per il giorno {booking.ServiceDate:dd/MM/yyyy HH:mm}.</p>
                <p><strong>Riepilogo richiesta:</strong></p>
                <ul>
                    <li><strong>Data:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</li>
                    {requestedDetails}
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

            string requestedDetails = "";
            if (booking.Tour != null) requestedDetails += $"<li><strong>Tour:</strong> {booking.Tour.Name}</li>";
            if (booking.ServiceType != null) requestedDetails += $"<li><strong>Servizio:</strong> {booking.ServiceType.Name}</li>";
            if (booking.Vehicle != null) requestedDetails += $"<li><strong>Veicolo:</strong> {booking.Vehicle.Name} ({booking.Vehicle.VehicleCategory?.Name})</li>";
            if (booking.PassengerOption != null) requestedDetails += $"<li><strong>Opzione Passeggeri:</strong> {booking.PassengerOption.Name}</li>";

            string body = $@"
                <h2>La tua prenotazione è stata accettata</h2>
                <p>Gentile {booking.FullName},</p>
                <p>siamo lieti di comunicarti che la tua richiesta di prenotazione è stata accettata.</p>
                <h3>Riepilogo della prenotazione:</h3>
                <ul>
                    <li><strong>Data Servizio:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</li>
                    {requestedDetails}
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

            string requestedDetails = "";
            if (booking.Tour != null) requestedDetails += $"<li><strong>Tour:</strong> {booking.Tour.Name}</li>";
            if (booking.ServiceType != null) requestedDetails += $"<li><strong>Servizio:</strong> {booking.ServiceType.Name}</li>";
            if (booking.Vehicle != null) requestedDetails += $"<li><strong>Veicolo:</strong> {booking.Vehicle.Name} ({booking.Vehicle.VehicleCategory?.Name})</li>";
            if (booking.PassengerOption != null) requestedDetails += $"<li><strong>Opzione Passeggeri:</strong> {booking.PassengerOption.Name}</li>";

            string body = $@"
                <h2>Informazioni sulla tua richiesta di prenotazione</h2>
                <p>Gentile {booking.FullName},</p>
                <p>ti informiamo che purtroppo non è stato possibile accettare la tua richiesta di prenotazione per il giorno {booking.ServiceDate:dd/MM/yyyy HH:mm}.</p>
                <p><strong>Riepilogo richiesta:</strong></p>
                <ul>
                    <li><strong>Data:</strong> {booking.ServiceDate:dd/MM/yyyy HH:mm}</li>
                    {requestedDetails}
                    <li><strong>Passeggeri:</strong> {booking.MaxSeats ?? 1}</li>
                </ul>
                <p><strong>Motivazione del rifiuto:</strong></p>
                <blockquote style='border-left: 5px solid #ccc; padding-left: 10px;'>{reason}</blockquote>
                <p>Rimaniamo a tua disposizione per ulteriori necessità.</p>
                <p>Cordiali saluti,<br/>Lo staff di Lia NCC</p>
            ";

            await SendEmailHtmlAsync(booking.Email, subject, body, "Booking", booking.Id);
        }

        public async Task SendReplyEmailAsync(string toEmail, string subject, string body, List<(string FileName, byte[] Content, string ContentType)>? attachments = null, string? relatedEntityName = null, Guid? relatedEntityId = null)
        {
            var attachmentFiles = new List<AttachmentFile>();
            var tempFiles = new List<string>();

            try
            {
                if (attachments != null)
                {
                    foreach (var att in attachments)
                    {
                        var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "_" + att.FileName);
                        await File.WriteAllBytesAsync(tempPath, att.Content);
                        tempFiles.Add(tempPath);

                        attachmentFiles.Add(new AttachmentFile
                        {
                            FilePath = tempPath,
                            DisplayName = att.FileName,
                            MediaType = att.ContentType,
                            Inline = false
                        });
                    }
                }

                await SendEmailInternalAsync(toEmail, subject, body, attachmentFiles, null, relatedEntityName, relatedEntityId);
            }
            finally
            {
                // Note: Ideally, delete files after mailer.SendEmailHtmlAsync is done.
                // Since SendEmailInternalAsync is awaited, it should be safe here.
                foreach (var file in tempFiles)
                {
                    try { if (File.Exists(file)) File.Delete(file); } catch { }
                }
            }
        }

        public async Task SendEmailHtmlAsync(string toEmail, string subject, string htmlBody, string? relatedEntityName = null, Guid? relatedEntityId = null)
        {
            await SendEmailInternalAsync(toEmail, subject, htmlBody, null, null, relatedEntityName, relatedEntityId);
        }

        private async Task SendEmailInternalAsync(string toEmail, string subject, string htmlBody, IEnumerable<AttachmentFile>? attachments, IEnumerable<InlineImage>? inlineImages, string? relatedEntityName, Guid? relatedEntityId)
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

                await mailer.SendEmailHtmlAsync(
                    toEmail,
                    subject,
                    htmlBody,
                    inlineImages,
                    attachments
                );

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

                throw;
            }
        }
    }
}
