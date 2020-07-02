using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ApiResource = IdentityServer4.Models.ApiResource;
using Client = IdentityServer4.Models.Client;
using IdentityResource = IdentityServer4.Models.IdentityResource;

namespace IdentityServerPOC
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("digitalRecipePoc", "Digital Recipe API")
                {
                    Scopes = { new Scope("digitalRecipePoc.read"), new Scope("digitalRecipePoc.write") }
                },
                new ApiResource("userManagement", "User Management API")
                {
                    Scopes = { new Scope("userManagement.read") }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {

            var configuration = new ConfigurationBuilder()
                .AddJsonFile("clients.json")
                .Build();

            return configuration.GetSection("Clients").Get<Client[]>();
        }
    }
}
