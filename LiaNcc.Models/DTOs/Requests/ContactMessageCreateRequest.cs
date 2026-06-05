using System;
using System.ComponentModel.DataAnnotations;

namespace LiaNcc.Models.DTOs.Requests
{
    public class ContactMessageCreateRequest
    {
        [Required(ErrorMessage = "Il nome completo è obbligatorio")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Il messaggio è obbligatorio")]
        public string Message { get; set; } = string.Empty;
    }
}
