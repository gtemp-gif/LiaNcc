using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Requests
{
    public class CreateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
