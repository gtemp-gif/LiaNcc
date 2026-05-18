using System;
using LiaNcc.Models.Entities;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IVehiclesApiClient : IApiClient<Vehicle, Guid>
    {
    }
}
