namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using global::AspNetCore.IdentityServer.Mongo.Configuration;
    using global::AspNetCore.IdentityServer.Mongo.DbContexts;
    using global::AspNetCore.IdentityServer.Mongo.Interfaces;
    using global::AspNetCore.IdentityServer.Mongo.Services;
    using global::AspNetCore.IdentityServer.Mongo.TokenCleanup;

    internal static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Configuration DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext">The IConfigurationDbContext to use.</typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        internal static IServiceCollection AddConfigurationDbContext<TContext>(this IServiceCollection services, Action<ConfigurationStoreOptions>? storeOptionsAction = null) where TContext : class, IConfigurationDbContext
        {
            var options = new ConfigurationStoreOptions();
            services.AddSingleton(options);
            storeOptionsAction?.Invoke(options);

            services.AddScoped<IConfigurationDbContext, TContext>();

            return services;
        }

        /// <summary>
        /// Add Operational DbContext to the DI system.
        /// </summary>
        /// <typeparam name="TContext">The IConfigurationDbContext to use.</typeparam>
        /// <param name="services"></param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        internal static IServiceCollection AddOperationalDbContext<TContext>(this IServiceCollection services, Action<OperationalStoreOptions>? storeOptionsAction = null) where TContext : class, IPersistedGrantDbContext
        {
            var storeOptions = new OperationalStoreOptions();
            services.AddSingleton(storeOptions);
            storeOptionsAction?.Invoke(storeOptions);

            services.AddScoped<IPersistedGrantDbContext, TContext>();
            services.AddTransient<TokenCleanupService>();

            return services;
        }

        /// <summary>
        /// Add Job Scheduler DbContext to the DI system.
        /// </summary>
        /// <param name="services"></param>
        internal static IServiceCollection AddJobSchedulerDbContext(this IServiceCollection services)
        {
            services.AddScoped<IJobSchedulerDbContext, JobSchedulerDbContext>();

            return services;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to the DI system.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        internal static IServiceCollection AddOperationalStoreNotification<T>(this IServiceCollection services) where T : class, IOperationalStoreNotification
        {
            services.AddTransient<IOperationalStoreNotification, T>();
            return services;
        }
    }
}
