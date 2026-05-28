using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int TotalVehicles { get; set; }
        public int BookableVehicles { get; set; }
        public int TotalTours { get; set; }
        public int ActiveTours { get; set; }
        public int TotalBookings { get; set; }
        public int PendingBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int TotalMessages { get; set; }
        public int UnreadMessages { get; set; }
        public int ActivePartners { get; set; }
        public int TotalMediaAssets { get; set; }

        public List<DashboardChartItemDto> BookingsByMonth { get; set; } = new();
        public List<DashboardChartItemDto> BookingsByStatus { get; set; } = new();
        public List<DashboardChartItemDto> ServicesByStatus { get; set; } = new();
        public List<DashboardChartItemDto> VehiclesByBookableStatus { get; set; } = new();
        public List<DashboardChartItemDto> ToursByStatus { get; set; } = new();

        public List<DashboardLatestBookingDto> LatestBookings { get; set; } = new();
        public List<DashboardLatestMessageDto> LatestMessages { get; set; } = new();
    }

    public class DashboardChartItemDto
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class DashboardLatestBookingDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ServiceDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class DashboardLatestMessageDto
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CompanyContactDto
    {
        public Guid Id { get; set; }
        public Guid CompanyId { get; set; }
        public string? Type { get; set; }
        public string? Value { get; set; }
        public bool IsPrimary { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
