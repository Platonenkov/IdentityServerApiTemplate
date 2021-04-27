using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerApiTemplate.Entities.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IdentityServerApiTemplate.Data.DatabaseInitialization
{
    /// <summary>
    /// Database Initializer
    /// </summary>
    public static class DatabaseInitializer
    {
        public static async void Seed(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            await scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.MigrateAsync();

            await using var config_context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            await config_context.Database.MigrateAsync();

            if (!config_context.Clients.Any())
            {
                foreach (var client in IdentityServerConfig.GetClients())
                {
                    await config_context.Clients.AddAsync(client.ToEntity());
                }
                await config_context.SaveChangesAsync();
            }

            if (!config_context.IdentityResources.Any())
            {
                foreach (var resource in IdentityServerConfig.GetIdentityResources())
                {
                    await config_context.IdentityResources.AddAsync(resource.ToEntity());
                }
                await config_context.SaveChangesAsync();
            }

            if (!config_context.ApiScopes.Any())
            {
                foreach (var resource in IdentityServerConfig.GetAPiScopes())
                {
                    await config_context.ApiScopes.AddAsync(resource.ToEntity());
                }
                await config_context.SaveChangesAsync();
            }

            if (!config_context.ApiResources.Any())
            {
                foreach (var resource in IdentityServerConfig.GetApiResources())
                {
                    await config_context.ApiResources.AddAsync(resource.ToEntity());
                }
                await config_context.SaveChangesAsync();
            }

            await using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            #region Roles

            var roles = AppData.Roles.ToArray();

            foreach (var role in roles)
            {
                var role_manager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                if (!context.Roles.Any(r => r.Name == role))
                {
                    await role_manager.CreateAsync(new ApplicationRole { Name = role, NormalizedName = role.ToUpper() });
                }
            }

            #endregion

            #region developer

            var developer1 = new ApplicationUser
            {
                Email = "Admin@base.com",
                NormalizedEmail = "ADMIN@BASE.COM",
                UserName = "Admin",
                FirstName = "Admin",
                LastName = "Admin",
                NormalizedUserName = "ADMIN",
                PhoneNumber = "+79000000000",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D"),
                ApplicationUserProfile = new ApplicationUserProfile
                {
                    CreatedAt = DateTime.Now,
                    CreatedBy = "SEED",
                    Permissions = new List<MicroservicePermission>
                    {
                        new MicroservicePermission
                        {
                            CreatedAt = DateTime.Now,
                            CreatedBy = "SEED",
                            PolicyName = "Logs:UserRoles:View",
                            Description = "Access policy for Logs controller user view"
                        }
                    }
                }
            };

            if (!context.Users.Any(u => u.UserName == developer1.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(developer1, "admin");
                developer1.PasswordHash = hashed;
                var user_store = scope.ServiceProvider.GetRequiredService<ApplicationUserStore>();
                var result = await user_store.CreateAsync(developer1);
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException("Cannot create account");
                }

                var user_manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                foreach (var role in roles)
                {
                    var role_added = await user_manager.AddToRoleAsync(developer1, role);
                    if (role_added.Succeeded)
                    {
                        await context.SaveChangesAsync();
                    }
                }
            }
            #endregion

            await context.SaveChangesAsync();


        }
    }
}