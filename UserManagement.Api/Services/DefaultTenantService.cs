using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UserManagement.Api.Dtos;

namespace UserManagement.Api.Services
{
    public class DefaultTenantService : ITenantService
    {
        private readonly HttpClient _httpClient;
        private const string BaseAddress = "http://localhost:5020/api/Tenant";

        public DefaultTenantService()
        {
            _httpClient = new HttpClient();
        }
        public Task Update(TenantDto tenant)
        {
            var content = new StringContent(JsonConvert.SerializeObject(tenant), Encoding.UTF8, "application/json");

            return _httpClient.PutAsync(BaseAddress, content);

        }

        public Task Create(TenantDto tenant)
        {
            var content = new StringContent(JsonConvert.SerializeObject(tenant), Encoding.UTF8, "application/json");

            return _httpClient.PostAsync(BaseAddress, content);
        }

        public async Task<TenantDto> FindById(string id)
        {
            var response = await _httpClient.GetAsync($"{BaseAddress}/{id}");
            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TenantDto>(jsonString);
        }

        public async Task<List<TenantDto>> GetAll()
        {
            var response = await _httpClient.GetAsync(BaseAddress);
            var jsonString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<TenantDto>>(jsonString);
        }
    }
}