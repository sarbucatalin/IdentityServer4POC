namespace UserManagement.Api.Dtos
{
    public class TenantDto
    {
        public string Id { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
    }
}