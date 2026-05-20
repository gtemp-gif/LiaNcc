using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiaNcc.Models.DTOs.Dashboard;
using LiaNcc.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly LiaNccDbContext _context;

        public DashboardRepository(LiaNccDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var summary = new DashboardSummaryDto();

            // Card Statistics
            summary.TotalServices = await _context.Services.CountAsync();
            summary.ActiveServices = await _context.Services.CountAsync(s => s.IsActive);

            summary.TotalVehicles = await _context.Vehicles.CountAsync();
            summary.BookableVehicles = await _context.Vehicles.CountAsync(v => v.IsBookable);

            summary.TotalTours = await _context.Tours.CountAsync();
            summary.ActiveTours = await _context.Tours.CountAsync(t => t.IsActive);

            summary.TotalBookings = await _context.Bookings.CountAsync();
            summary.PendingBookings = await _context.Bookings.CountAsync(b => b.Status == "Pending");
            summary.ConfirmedBookings = await _context.Bookings.CountAsync(b => b.Status == "Confirmed");
            summary.CancelledBookings = await _context.Bookings.CountAsync(b => b.Status == "Cancelled");

            summary.TotalMessages = await _context.ContactMessages.CountAsync();
            summary.UnreadMessages = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            summary.ActivePartners = await _context.Partners.CountAsync(p => p.IsActive);
            summary.TotalMediaAssets = await _context.MediaAssets.CountAsync();

            // Bookings By Month (Last 12 months)
            var twelveMonthsAgo = DateTime.UtcNow.AddMonths(-11);
            twelveMonthsAgo = new DateTime(twelveMonthsAgo.Year, twelveMonthsAgo.Month, 1);

            var bookingsPerMonth = await _context.Bookings
                .Where(b => b.CreatedAt >= twelveMonthsAgo)
                .GroupBy(b => new { b.CreatedAt.Year, b.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
                .ToListAsync();

            for (int i = 0; i < 12; i++)
            {
                var date = twelveMonthsAgo.AddMonths(i);
                var match = bookingsPerMonth.FirstOrDefault(m => m.Year == date.Year && m.Month == date.Month);
                summary.BookingsByMonth.Add(new DashboardChartItemDto
                {
                    Label = date.ToString("MMM yyyy"),
                    Value = match?.Count ?? 0
                });
            }

            // Bookings By Status
            summary.BookingsByStatus = await _context.Bookings
                .GroupBy(b => b.Status)
                .Select(g => new DashboardChartItemDto { Label = g.Key, Value = g.Count() })
                .ToListAsync();

            // Services By Status
            summary.ServicesByStatus = new List<DashboardChartItemDto>
            {
                new DashboardChartItemDto { Label = "Active", Value = summary.ActiveServices },
                new DashboardChartItemDto { Label = "Inactive", Value = summary.TotalServices - summary.ActiveServices }
            };

            // Vehicles By Bookable Status
            summary.VehiclesByBookableStatus = new List<DashboardChartItemDto>
            {
                new DashboardChartItemDto { Label = "Bookable", Value = summary.BookableVehicles },
                new DashboardChartItemDto { Label = "Not Bookable", Value = summary.TotalVehicles - summary.BookableVehicles }
            };

            // Tours By Status
            summary.ToursByStatus = new List<DashboardChartItemDto>
            {
                new DashboardChartItemDto { Label = "Active", Value = summary.ActiveTours },
                new DashboardChartItemDto { Label = "Inactive", Value = summary.TotalTours - summary.ActiveTours }
            };

            // Latest Bookings
            summary.LatestBookings = await _context.Bookings
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .Select(b => new DashboardLatestBookingDto
                {
                    Id = b.Id,
                    FullName = b.FullName,
                    Email = b.Email,
                    ServiceDate = b.ServiceDate,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt
                })
                .ToListAsync();

            // Latest Messages
            summary.LatestMessages = await _context.ContactMessages
                .OrderByDescending(m => m.CreatedAt)
                .Take(5)
                .Select(m => new DashboardLatestMessageDto
                {
                    Id = m.Id,
                    FullName = m.FullName,
                    Email = m.Email,
                    IsRead = m.IsRead,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return summary;
        }
    }
}
