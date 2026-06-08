using System;

namespace LiaNcc.Models.DTOs.Requests
{
    public class ContactMessageFilterRequest
    {
        public bool? IsRead { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchText { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}
