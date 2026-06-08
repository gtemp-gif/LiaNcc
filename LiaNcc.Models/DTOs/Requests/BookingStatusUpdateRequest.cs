using System;

namespace LiaNcc.Models.DTOs.Requests
{
    public class BookingStatusUpdateRequest
    {
        public string Status { get; set; } = string.Empty;
        public string? Note { get; set; }
    }
}
