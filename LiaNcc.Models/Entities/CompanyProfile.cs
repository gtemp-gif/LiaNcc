using System;
using System.Collections.Generic;

namespace LiaNcc.Models.Entities
{
    public class CompanyProfile
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? VatNumber { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? GoogleMapsUrl { get; set; }
        public string? AboutTitle { get; set; }
        public string? AboutDescription { get; set; }
        public string? AboutImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<CompanyContact> CompanyContacts { get; set; } = new List<CompanyContact>();
    }
}
