using System;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;
using Microsoft.AspNetCore.Http;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface IMediaAssetsApiClient : IApiClient<MediaAsset, Guid>
    {
        Task<MediaAsset> UploadMediaAsync(IFormFile file);
    }
}
