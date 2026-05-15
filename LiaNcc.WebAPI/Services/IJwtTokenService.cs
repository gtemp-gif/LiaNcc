using System.Collections.Generic;
using LiaNcc.Models.Entities;

namespace LiaNcc.WebAPI.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, IEnumerable<Role> roles);
    }
}
