using System;

namespace LiaNcc.Models.Entities
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime ServiceDate { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? PassengerOptionId { get; set; }
        public Guid? TourId { get; set; }
        public Guid? VehicleId { get; set; }
        public string? Message { get; set; }
        public int? MaxSeats { get; set; }
        public string Status { get; set; } = "Pending";
        public string? SourcePage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public BookingServiceType? ServiceType { get; set; }
        public BookingPassengerOption? PassengerOption { get; set; }
        public Tour? Tour { get; set; }
        public Vehicle? Vehicle { get; set; }
    }
}
