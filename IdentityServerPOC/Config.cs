﻿using IdentityServer4.Models;
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
                new ApiResource("digitalrecipiepoc", "Digital Recipie API")
                {
                    Scopes = {new Scope("api.read")}
                }
                //,new ApiResource("identityServer", "Identity Server API")
                //{
                //    Scopes = {new Scope("api.read")}
                //}
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                new Client {
                    RequireConsent = false,
                    ClientId = "angular_spa",
                    ClientName = "Angular SPA",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowedScopes = { "openid", "profile", "email", "api.read" },
                    RedirectUris = {"http://localhost:4200/auth-callback","http://localhost:4200/signin-callback"},
                    PostLogoutRedirectUris = {"http://localhost:4200/","http://localhost:4200/signout-callback"},
                    AllowedCorsOrigins = {"http://localhost:4200"},
                    AllowAccessTokensViaBrowser = true,
                    AccessTokenLifetime = 3600
                }
            };
        }
    }
}
