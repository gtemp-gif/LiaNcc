using System;
using LiaNcc.Models.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LiaNcc.BO.Services.Interfaces
{
    public interface ICompanyApiClient : IApiClient<CompanyProfile, Guid>
    {
        Task<CompanyProfile?> GetFirstCompanyProfileAsync();
        Task<IEnumerable<CompanyContact>> GetCompanyContactsAsync();
        Task CreateCompanyContactAsync(CompanyContact contact);
        Task UpdateCompanyContactAsync(Guid id, CompanyContact contact);
        Task DeleteCompanyContactAsync(Guid id);
    }
}
