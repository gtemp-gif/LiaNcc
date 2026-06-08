using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.WebAPI.Services
{
    public interface IMailService
    {
        Task SendContactNotificationAsync(ContactMessage contactMessage);
        Task SendContactCustomerConfirmationAsync(ContactMessage contactMessage);
        Task SendBookingNotificationAsync(Booking booking);
        Task SendBookingCustomerConfirmationAsync(Booking booking);
        Task SendBookingAcceptedAsync(Booking booking);
        Task SendBookingRejectedAsync(Booking booking, string reason);
        Task SendReplyEmailAsync(string toEmail, string subject, string body, List<(string FileName, byte[] Content, string ContentType)>? attachments = null, string? relatedEntityName = null, Guid? relatedEntityId = null);
        Task SendEmailHtmlAsync(string toEmail, string subject, string htmlBody, string? relatedEntityName = null, Guid? relatedEntityId = null);
    }
}
