using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace AuthServer
{
    public class Config
    {
        private static string spaClientUrl = "https://localhost:4200";
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),

            };
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API"),
                new ApiResource("api2", "My API 2")
            };
        }
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                // backward compat
                new ApiScope("api1"),
                new ApiScope("api2"),
                new ApiScope("api3")
            };
        }
        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    // scopes that client has access to
                    AllowedScopes = { "api1", "api2" },

                },
                new Client
                {
                    ClientId = "spaCodeClient",
                    ClientName = "SPA Code Client",
                    AccessTokenType = AccessTokenType.Jwt,
                    // RequireConsent = false,
                    AccessTokenLifetime = 60,// 330 seconds, default 60 minutes
                    IdentityTokenLifetime = 30,

                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    { 
                        $"https://localhost:4200/callback",
                        "https://localhost:4200/",
                        "https://localhost:4200/silent-renew.html",
                        //$"{spaClientUrl}/silent-renew.html",
                        //"https://localhost:4200",
                      
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:4200/",
                        "https://localhost:4200/unauthorized"

                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:4200"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                },
                new Client
                {
                    ClientId = "spaCodeClient2",
                    ClientName = "SPA Code Client",
                    AccessTokenType = AccessTokenType.Jwt,
                    // RequireConsent = false,
                    AccessTokenLifetime = 60,// 330 seconds, default 60 minutes
                    IdentityTokenLifetime = 30,

                    RequireClientSecret = false,
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,

                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>
                    {
                        "https://localhost:4600/",
                        $"https://localhost:4600/callback",
                        "https://localhost:4600/silent-renew.html"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:4600/",
                        "https://localhost:4600/unauthorized"

                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:4600"
                    },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api3"
                    }
                },
            };
        }
    }
}
