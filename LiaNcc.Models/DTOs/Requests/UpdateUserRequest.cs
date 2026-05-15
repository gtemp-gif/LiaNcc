using System;
using System.Collections.Generic;

namespace LiaNcc.Models.DTOs.Requests
{
    public class UpdateUserRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
