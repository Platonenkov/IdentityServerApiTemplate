using Calabonga.EntityFrameworkCore.Entities.Base;
using Calabonga.UnitOfWork;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServerApiTemplate.Data.Base
{
    /// <summary>
    /// Base DbContext with predefined configuration
    /// </summary>
    public abstract class DbContextBase : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        private const string __DefaultUserName = "Anonymous";

        protected DbContextBase(DbContextOptions options) : base(options) => LastSaveChangesResult = new SaveChangesResult();

        public SaveChangesResult LastSaveChangesResult { get; }


        /// <summary>
        ///     Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        ///     Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        ///     been sent successfully to the database.
        /// </param>
        /// <remarks>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                DbSaveChanges();
                return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            }
            catch (Exception exception)
            {
                LastSaveChangesResult.Exception = exception;
                return Task.FromResult(0);
            }
        }
        
        /// <summary>
        ///     Saves all changes made in this context to the database.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">
        ///     Indicates whether <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AcceptAllChanges" /> is called after the changes have
        ///     been sent successfully to the database.
        /// </param>
        /// <remarks>
        ///     This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///     changes to entity instances before saving to the underlying database. This can be disabled via
        ///     <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        /// </remarks>
        /// <returns>
        ///     The number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            try
            {
                DbSaveChanges();
                return base.SaveChanges(acceptAllChangesOnSuccess);
            }
            catch (Exception exception)
            {
                LastSaveChangesResult.Exception = exception;
                return 0;
            }
        }

        public override int SaveChanges()
        {
            try
            {
                DbSaveChanges();
                return base.SaveChanges();
            }
            catch (Exception exception)
            {
                LastSaveChangesResult.Exception = exception;
                return 0;
            }
        }

        /// <summary>
        ///     Asynchronously saves all changes made in this context to the database.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         This method will automatically call <see cref="M:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.DetectChanges" /> to discover any
        ///         changes to entity instances before saving to the underlying database. This can be disabled via
        ///         <see cref="P:Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker.AutoDetectChangesEnabled" />.
        ///     </para>
        ///     <para>
        ///         Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///         that any asynchronous operations have completed before calling another method on this context.
        ///     </para>
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous save operation. The task result contains the
        ///     number of state entries written to the database.
        /// </returns>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateException">
        ///     An error is encountered while saving to the database.
        /// </exception>
        /// <exception cref="T:Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException">
        ///     A concurrency violation is encountered while saving to the database.
        ///     A concurrency violation occurs when an unexpected number of rows are affected during save.
        ///     This is usually because the data in the database has been modified since it was loaded into memory.
        /// </exception>
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                DbSaveChanges();
                return base.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
                LastSaveChangesResult.Exception = exception;
                return Task.FromResult(0);
            }
        }

        private void DbSaveChanges()
        {
            var created_entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Added);
            foreach (var entry in created_entries)
            {
                if (!(entry.Entity is IAuditable))
                {
                    continue;
                }

                var creation_date = DateTime.Now.ToUniversalTime();
                var user_name = entry.Property("CreatedBy").CurrentValue == null
                    ? __DefaultUserName
                    : entry.Property("CreatedBy").CurrentValue;
                var updated_at = entry.Property("UpdatedAt").CurrentValue;
                var created_at = entry.Property("CreatedAt").CurrentValue;
                if (created_at != null)
                {
                    if (DateTime.Parse(created_at.ToString()).Year > 1970)
                    {
                        entry.Property("CreatedAt").CurrentValue = ((DateTime)created_at).ToUniversalTime();
                    }
                    else
                    {
                        entry.Property("CreatedAt").CurrentValue = creation_date;
                    }
                }
                else
                {
                    entry.Property("CreatedAt").CurrentValue = creation_date;
                }

                if (updated_at != null)
                {
                    if (DateTime.Parse(updated_at.ToString()).Year > 1970)
                    {
                        entry.Property("UpdatedAt").CurrentValue = ((DateTime)updated_at).ToUniversalTime();
                    }
                    else
                    {
                        entry.Property("UpdatedAt").CurrentValue = creation_date;
                    }
                }
                else
                {
                    entry.Property("UpdatedAt").CurrentValue = creation_date;
                }

                entry.Property("CreatedBy").CurrentValue = user_name;
                entry.Property("UpdatedBy").CurrentValue = user_name;

                LastSaveChangesResult.AddMessage($"ChangeTracker has new entities: {entry.Entity.GetType()}");
            }

            var updated_entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
            foreach (var entry in updated_entries)
            {
                if (entry.Entity is IAuditable)
                {
                    var user_name = entry.Property("UpdatedBy").CurrentValue == null
                        ? __DefaultUserName
                        : entry.Property("UpdatedBy").CurrentValue;
                    entry.Property("UpdatedAt").CurrentValue = DateTime.Now.ToUniversalTime();
                    entry.Property("UpdatedBy").CurrentValue = user_name;
                }

                LastSaveChangesResult.AddMessage($"ChangeTracker has modified entities: {entry.Entity.GetType()}");
            }
        }

        /// <summary>
        /// Configures the schema needed for the identity framework.
        /// </summary>
        /// <param name="builder">
        /// The builder being used to construct the model for this context.
        /// </param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var apply_generic_method = typeof(ModelBuilder).GetMethods(BindingFlags.Instance | BindingFlags.Public).First(x => x.Name == "ApplyConfiguration");
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(c => c.IsClass && !c.IsAbstract && !c.ContainsGenericParameters))
            {
                foreach (var item in type.GetInterfaces())
                {
                    if (!item.IsConstructedGenericType || item.GetGenericTypeDefinition() != typeof(IEntityTypeConfiguration<>))
                    {
                        continue;
                    }

                    var apply_concrete_method = apply_generic_method.MakeGenericMethod(item.GenericTypeArguments[0]);
                    apply_concrete_method.Invoke(builder, new[] { Activator.CreateInstance(type) });
                    break;
                }
            }

            builder.EnableAutoHistory(2048);
        }
    }
}