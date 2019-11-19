﻿using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankOfDotNet.IdentityServer
{
    public static class Config
    {

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "valonso",
                    Password =  "password"
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password =  "password"
                },
            };
        }

        public static IEnumerable<ApiResource> GetAllApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("bankOfDotNetApi", "Customer API for BankOfDotNet")
            };
        }


        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                // Client-Credential based grant type
                new Client
                {
                    ClientId = "client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    AllowedScopes =  { "bankOfDotNetApi" }
                },
                // Resource Owner Password Grant Type
                new Client
                {
                     ClientId = "ro.client",
                      AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                      ClientSecrets = { new Secret("secret".Sha256()) },
                    AllowedScopes =  { "bankOfDotNetApi" }
                },

                // Implicit Flow Grant Type
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:5003/signin-oidc" },
                     PostLogoutRedirectUris = { "http://localhost:5003/signout-callback-oidc" },
                     AllowedScopes =  new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    }
                },

                // Implicit Flow Grant Type
                new Client
                {
                    ClientId = "swagger",
                    ClientName = "My Swagger API UI",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:5001/swagger/oauth2-redirect.html/" },
                     PostLogoutRedirectUris = { "http://localhost:5001/swagger" },
                     AllowedScopes =  new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "bankOfDotNetApi"
                    },
                     AllowAccessTokensViaBrowser = true
                }

            };
        }



    }
}
