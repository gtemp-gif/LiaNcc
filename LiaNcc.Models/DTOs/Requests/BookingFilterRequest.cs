using System;

namespace LiaNcc.Models.DTOs.Requests
{
    public class BookingFilterRequest
    {
        public string? Status { get; set; }
        public Guid? ServiceTypeId { get; set; }
        public Guid? TourId { get; set; }
        public Guid? VehicleId { get; set; }
        public DateTime? FromServiceDate { get; set; }
        public DateTime? ToServiceDate { get; set; }
        public DateTime? FromCreatedAt { get; set; }
        public DateTime? ToCreatedAt { get; set; }
        public string? SearchText { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
