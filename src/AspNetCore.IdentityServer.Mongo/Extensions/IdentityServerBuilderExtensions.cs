namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using global::AspNetCore.IdentityServer.Mongo.Configuration;
    using global::AspNetCore.IdentityServer.Mongo.DbContexts;
    using global::AspNetCore.IdentityServer.Mongo.Interfaces;
    using global::AspNetCore.IdentityServer.Mongo.Services;
    using global::AspNetCore.IdentityServer.Mongo.Stores;
    using global::AspNetCore.IdentityServer.Mongo.TokenCleanup;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Extension methods to add MongoDB database support to IdentityServer.
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Configures MongoDB implementation of IClientStore, IResourceStore, and ICorsPolicyService with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStore(this IIdentityServerBuilder builder, Action<ConfigurationStoreOptions>? storeOptionsAction = null)
            => builder.AddConfigurationStore<ConfigurationDbContext>(storeOptionsAction);

        /// <summary>
        /// Configures MongoDB implementation of IClientStore, IResourceStore, and ICorsPolicyService with IdentityServer.
        /// </summary>
        /// <typeparam name="TContext">The IConfigurationDbContext to use.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStore<TContext>(this IIdentityServerBuilder builder, Action<ConfigurationStoreOptions>? storeOptionsAction = null) where TContext : class, IConfigurationDbContext
        {
            builder.Services.AddConfigurationDbContext<TContext>(storeOptionsAction);

            builder.AddClientStore<ClientStore>();
            builder.AddResourceStore<ResourceStore>();
            builder.AddCorsPolicyService<CorsPolicyService>();
            builder.AddIdentityProviderStore<IdentityProviderStore>();

            return builder;
        }

        /// <summary>
        /// Configures caching for IClientStore, IResourceStore, and ICorsPolicyService with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddConfigurationStoreCache(this IIdentityServerBuilder builder)
        {
            builder.AddInMemoryCaching();

            // add the caching decorators
            builder.AddClientStoreCache<ClientStore>();
            builder.AddResourceStoreCache<ResourceStore>();
            builder.AddCorsPolicyCache<CorsPolicyService>();
            builder.AddIdentityProviderStoreCache<IdentityProviderStore>();

            return builder;
        }

        /// <summary>
        /// Configures MongoDB implementation of IPersistedGrantStore with IdentityServer.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOperationalStore(this IIdentityServerBuilder builder, Action<OperationalStoreOptions>? storeOptionsAction = null)
            => builder.AddOperationalStore<PersistedGrantDbContext>(storeOptionsAction);

        /// <summary>
        /// Configures MongoDB implementation of IPersistedGrantStore with IdentityServer.
        /// </summary>
        /// <typeparam name="TContext">The IPersistedGrantDbContext to use.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="storeOptionsAction">The store options action.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOperationalStore<TContext>(this IIdentityServerBuilder builder, Action<OperationalStoreOptions>? storeOptionsAction = null) where TContext : class, IPersistedGrantDbContext
        {
            builder.Services.AddOperationalDbContext<TContext>(storeOptionsAction);
            builder.AddSigningKeyStore<SigningKeyStore>();
            builder.AddPersistedGrantStore<PersistedGrantStore>();
            builder.AddDeviceFlowStore<DeviceFlowStore>();

            builder.Services.AddHostedService<ConfigureMongoDbIndexesService>();

            builder.Services.AddSingleton<IHostedService, TokenCleanupHost>();
            builder.Services.AddJobSchedulerDbContext();

            return builder;
        }

        /// <summary>
        /// Adds an implementation of the IOperationalStoreNotification to IdentityServer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddOperationalStoreNotification<T>(this IIdentityServerBuilder builder) where T : class, IOperationalStoreNotification
        {
            builder.Services.AddOperationalStoreNotification<T>();
            return builder;
        }
    }
}
