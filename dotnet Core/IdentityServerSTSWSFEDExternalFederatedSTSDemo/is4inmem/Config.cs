// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.WsFederation;
using IdentityServer4.WsFederation.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace is4inmem
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[]
            {
                new ApiResource("api1", "My API #1")
            };

        // This is the Identity Server defination of a client.
        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "https://mvc",
                    AllowedScopes = { "openid", "profile" },
                    RedirectUris = { "https://localhost:5001/signin-wsfed" },
                    ProtocolType = IdentityServerConstants.ProtocolTypes.WsFederation
                }
            };

        // These are the WS-Fed specific settings for a given relying party.
        // There must be a matching client with the same name, i.e. https://mvc
        public static IEnumerable<RelyingParty> RelyingParties =>
            new RelyingParty[]
            {
                new RelyingParty
                {
                    Realm = "https://mvc",
                    TokenType = WsFederationConstants.TokenTypes.Saml11TokenProfile11,
                    ClaimMapping = new Dictionary<string, string> { { JwtClaimTypes.Name, ClaimTypes.Name } }
                }
            };
    }
}