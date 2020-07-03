using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Api.Dtos;

namespace UserManagement.Api.Services
{
    public interface ITenantService
    {
        Task Update(TenantDto tenant);
        Task Create(TenantDto tenant);
        Task<TenantDto> FindById(string id);
        Task<List<TenantDto>> GetAll();
    }
}