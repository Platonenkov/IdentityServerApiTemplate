using Calabonga.UnitOfWork.Controllers.DependencyContainer;
using IdentityServerApiTemplate.Server.AppStart.Configures;
using IdentityServerApiTemplate.Server.AppStart.ConfigureServices;
using IdentityServerApiTemplate.Server.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServerApiTemplate.Server
{
    /// <summary>
    /// Entry point
    /// </summary>
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            Environment = environment;
            Configuration = configuration;
        }

        /// <summary> Configuration </summary>
        public IConfiguration Configuration { get; }
        /// <summary> Host </summary>
        public IHostEnvironment Environment { get; }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesBase.ConfigureServices(services, Configuration);
            ConfigureServicesAuthentication.ConfigureServices(services, Configuration, Environment);
            ConfigureServicesSwagger.ConfigureServices(services, Configuration);
            ConfigureServicesCors.ConfigureServices(services, Configuration);
            ConfigureServicesControllers.ConfigureServices(services);
            ConfigureServicesMediator.ConfigureServices(services);
            ConfigureServicesValidators.ConfigureServices(services);

            DependencyContainer.Common(services);
            NimbleDependencyContainer.ConfigureServices(services);
        }
        /// <summary>
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="mapper"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AutoMapper.IConfigurationProvider mapper)
        {
            ConfigureCommon.Configure(app, env, mapper);
            ConfigureAuthentication.Configure(app);
            ConfigureEndpoints.Configure(app);
        }
    }
}
