using System;

namespace LiaNcc.Models.DTOs.Responses
{
    public class BookingDetailDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime ServiceDate { get; set; }

        public Guid? ServiceTypeId { get; set; }
        public string? ServiceTypeName { get; set; }
        public string? ServiceTypeDescription { get; set; }

        public Guid? PassengerOptionId { get; set; }
        public string? PassengerOptionName { get; set; }
        public string? PassengerOptionDescription { get; set; }

        public Guid? TourId { get; set; }
        public string? TourName { get; set; }
        public string? TourDescription { get; set; }

        public Guid? VehicleId { get; set; }
        public string? VehicleName { get; set; }
        public string? VehicleCategoryName { get; set; }
        public string? VehicleDescription { get; set; }

        public int? MaxSeats { get; set; }
        public string? Message { get; set; }
        public string Status { get; set; } = "Pending";
        public string? SourcePage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
