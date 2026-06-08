using System;
using System.ComponentModel.DataAnnotations;

namespace LiaNcc.Models.DTOs.Requests
{
    public class BookingCreateRequest
    {
        [Required(ErrorMessage = "Il nome completo è obbligatorio")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [Required(ErrorMessage = "La data del servizio è obbligatoria")]
        public DateTime ServiceDate { get; set; }

        public Guid? ServiceTypeId { get; set; }
        public Guid? PassengerOptionId { get; set; }
        public Guid? TourId { get; set; }
        public Guid? VehicleId { get; set; }

        public string? Message { get; set; }
        public int? MaxSeats { get; set; }
        public string? SourcePage { get; set; }
    }
}
