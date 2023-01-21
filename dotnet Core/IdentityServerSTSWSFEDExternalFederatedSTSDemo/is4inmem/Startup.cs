// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Quickstart.UI;
using IdentityServer4.WsFederation;
using IdentityServer4.WsFederation.Configuration;
using IdentityServer4.WsFederation.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Xml;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace is4inmem
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration { get; }

        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme).AddAzureAD(options => Configuration.Bind("AzureAd", options));

            #region IIS Config Settings
            //// configures IIS out-of-proc settings (see https://github.com/aspnet/AspNetCore/issues/14882)
            //services.Configure<IISOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});

            //// configures IIS in-proc settings
            //services.Configure<IISServerOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = false;
            //});
            #endregion

            // Configuration example: https://github.com/RockSolidKnowledge/Samples.IdentityServer4.WsFederationIntegration/
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            });

            builder.AddTestUsers(TestUsers.Users);
            builder.AddInMemoryIdentityResources(Config.Ids);
            builder.AddInMemoryApiResources(Config.Apis);
            builder.AddInMemoryClients(Config.Clients);
            try
            {
                builder.AddSigningCredential(GetCertificateFromStore("CN=sign"));
            }
            catch (Exception)
            {

                throw new Exception("Signing certificate not found in LocalMachine personal store. Has the 'CreateSigningKey.ps1' script from the 'is4inmem' project been run?");
            }
            builder.AddWsFederationPlugin(options =>
            {
                // DefaultClaimMapping maps OpenID format claim types to SAML suitable claim types. Claim types not defined in the mapping will not be included in generated SAML tokens.
                //options.DefaultClaimMapping = new Dictionary<string, string>
                //{
                //    { JwtClaimTypes.Name, ClaimTypes.Name },
                //    { JwtClaimTypes.Subject, ClaimTypes.NameIdentifier },
                //    { JwtClaimTypes.Email, ClaimTypes.Email },
                //    { JwtClaimTypes.GivenName, ClaimTypes.GivenName },
                //    { JwtClaimTypes.FamilyName, ClaimTypes.Surname },
                //    { JwtClaimTypes.BirthDate, ClaimTypes.DateOfBirth },
                //    { JwtClaimTypes.WebSite, ClaimTypes.Webpage },
                //    { JwtClaimTypes.Gender, ClaimTypes.Gender },
                //    { JwtClaimTypes.Role, ClaimTypes.Role }
                //};
                //options.DefaultDigestAlgorithm = "http://www.w3.org/2001/04/xmlenc#sha256";
                //options.DefaultSamlNameIdentifierFormat = "urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified";
                //options.DefaultSignatureAlgorithm = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";
                //options.DefaultTokenType = "urn:oasis:names:tc:SAML:2.0:assertion";
                options.Licensee = "DEMO";
                options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjAtMDMtMjlUMDA6MDA6MDMuODc2MTQ5NiswMDowMCIsImlhdCI6IjIwMjAtMDItMjhUMDA6MDA6MDMiLCJvcmciOiJERU1PIiwiYXVkIjozfQ==.C6SW/wbNUFgW0J9QjWI8Sw/3MLnzR8YfcZfmnVJB1EO/nKzGRcRPBGuvV5h3dkR5sZ4S2r/vG9GxFgVK3tWhD85hT3WcihaZ81SqwKLvrKeYAaUNwC/C5uceRAY5j/kSvuWO1sJUgIbEZE+N9bDynOq/PUFCcP3g+qZPJyZJcX2ahH7XR40oUu2bOHIjLidd/UkPZ3EPP2yqLGjy9a58fgib6TDf4d0gQqLbcl6XRYaz4cZFPmQjpS22qRdt5cZP9Hvl1NGgpkl3dKOsGmrC0DeYhd4fU20HRPN03/Gn2URERxI0Ymxm9/hs1SmKuqMnxeYrEOTZDwWvYOqxPUwOF/GAa+hePbshPDE+s0+RmJWFK+45ZMa2ivReukTdRkiaFIf+oDoBltRpYyp9K3r19Xyxm4rRaa9jrHVhFcxBKayZQ1Bd5FWZ58kaRfirz4XDS6A/8eeaXWqtu9+gHNWQX1ggsjD0y4nWHrznawuhFvgQGWuj6WSMSxfE6ppTkFgRH7c4tro3FOwpmtL3zka+cvl+xFUfN2hCyLM069dZVrjbe632pPoqlUKI5LJM2c57GWcaoJ/DgfedeCgnEBGQtZJMjTD6HPkU9lAe1OFTgiOeQ2Q5QGZUP7htIMs3CoYtXpNWn1rIcJ8o5C50yUpN35cv1y0jGRWA24cMXvZQUCo=";
                //options.WsFederationEndpoint = "wsfed";
            });
            builder.AddInMemoryRelyingParties(Config.RelyingParties);
            builder.AddInMemoryIdentityResources(Config.Ids);
            builder.AddInMemoryApiResources(Config.Apis);
            builder.AddInMemoryClients(Config.Clients);
            // not recommended for production - you need to store your key material somewhere secure
            //builder.AddDeveloperSigningCredential();

            


        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
           

            app.UseRouting();
            app.UseIdentityServer().UseIdentityServerWsFederationPlugin();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }


        private X509Certificate2 GetCertificateFromStore(string certName)
        {

            // Get the certificate store for the local machine.
            X509Store store = new X509Store(StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);

                // Place all certificates in an X509Certificate2Collection object.
                X509Certificate2Collection certCollection = store.Certificates;
                // If using a certificate with a trusted root you do not need to FindByTimeValid, instead:
                // currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, true);
                X509Certificate2Collection currentCerts = certCollection.Find(X509FindType.FindByTimeValid, DateTime.Now, false);
                X509Certificate2Collection signingCert = currentCerts.Find(X509FindType.FindBySubjectDistinguishedName, certName, false);
                if (signingCert.Count == 0)
                    return null;
                // Return the first certificate in the collection, has the right name and is current.
                return signingCert[0];
            }
            finally
            {
                store.Close();
            }
        }



    }
}