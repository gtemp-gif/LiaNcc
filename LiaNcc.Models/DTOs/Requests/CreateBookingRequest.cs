using System;

namespace LiaNcc.Models.DTOs.Requests
{
    public class CreateBookingRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
