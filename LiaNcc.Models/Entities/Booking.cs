using System;

namespace LiaNcc.Models.Entities
{
    public class Booking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
