using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LiaNcc.Models.Entities;

namespace LiaNcc.Repository.Interfaces
{
    public interface ICompanyRepository
    {
        Task<CompanyProfile?> GetCompanyProfileAsync();
        Task<IEnumerable<CompanyContact>> GetCompanyContactsAsync();
        Task<CompanyContact?> GetContactByIdAsync(Guid id);
        Task<CompanyProfile> CreateOrUpdateProfileAsync(CompanyProfile profile);
        Task<CompanyContact> CreateContactAsync(CompanyContact contact);
        Task UpdateContactAsync(CompanyContact contact);
        Task DeleteContactAsync(Guid id);
    }
}
