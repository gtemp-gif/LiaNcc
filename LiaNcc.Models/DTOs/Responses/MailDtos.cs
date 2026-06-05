using System;

namespace LiaNcc.Models.DTOs.Responses
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime ServiceDate { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }
        public Guid? PassengerOptionId { get; set; }
        public string? PassengerOptionName { get; set; }
        public Guid? TourId { get; set; }
        public string? TourName { get; set; }
        public Guid? VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? Message { get; set; }
        public int? MaxSeats { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ContactMessageDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class MailSendResultDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
