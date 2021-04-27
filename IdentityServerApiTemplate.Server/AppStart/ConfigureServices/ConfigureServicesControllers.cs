using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerApiTemplate.Server.AppStart.ConfigureServices
{
    /// <summary>
    /// Configure controllers
    /// </summary>
    public static class ConfigureServicesControllers
    {
        /// <summary>
        /// Configure services
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureServices(IServiceCollection services) =>
            services.AddControllersWithViews()
               .AddRazorRuntimeCompilation();
    }
}
