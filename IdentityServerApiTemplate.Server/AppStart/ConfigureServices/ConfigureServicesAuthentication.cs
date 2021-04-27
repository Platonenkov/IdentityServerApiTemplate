using IdentityServer4.AccessTokenValidation;
using IdentityServerApiTemplate.Data;
using IdentityServerApiTemplate.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignedCertificate;
using System.Reflection;

namespace IdentityServerApiTemplate.Server.AppStart.ConfigureServices
{
    /// <summary>
    /// ASP.NET Core services registration and configurations
    /// Authentication path
    /// </summary>
    public static class ConfigureServicesAuthentication
    {
        /// <summary>
        /// Configure Authentication & Authorization
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            var url = configuration.GetSection("IdentityServer").GetValue<string>("Url");
            services.AddAuthentication()
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(
                    options =>
                    {
                        options.SupportedTokens = SupportedTokens.Jwt;
                        options.Authority = url;
                        options.EnableCaching = true;
                        options.RequireHttpsMetadata = false;
                    });

            var migration_assembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options =>
                {
                    options.Authentication.CookieSlidingExpiration = true;
                    options.IssuerUri = url;
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.UserInteraction.LoginUrl = "/Authentication/Login";
                    options.UserInteraction.LogoutUrl = "/Authentication/Logout";
                })
               .AddAspNetIdentity<ApplicationUser>()
                // this adds the config data from DB (clients, resources, CORS)
                .AddConfigurationStore(
                    o =>
                    {
                        o.ConfigureDbContext = b => b.UseSqlServer(
                            configuration.GetConnectionString(nameof(ApplicationDbContext)),
                            sql => sql.MigrationsAssembly(migration_assembly));
                    })
               // this adds the operational data from DB (codes, tokens, consents)
               .AddOperationalStore(
                    o =>
                    {
                        o.ConfigureDbContext = b => b.UseSqlServer(
                            configuration.GetConnectionString(nameof(ApplicationDbContext)),
                            sql => sql.MigrationsAssembly(migration_assembly));
                    })
                .AddJwtBearerClientAuthentication()
                .AddProfileService<IdentityProfileService>()
                .AddSigningCredential(Certificate.CreateCertificate("IdentityServerCertificate", "ApiP@$$w0rd", 1));
        }
    }
}
