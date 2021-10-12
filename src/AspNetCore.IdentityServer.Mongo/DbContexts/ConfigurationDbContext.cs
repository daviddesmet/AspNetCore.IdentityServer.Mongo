namespace AspNetCore.IdentityServer.Mongo.DbContexts
{
    using System;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// DbContext for the IdentityServer configuration data.
    /// </summary>
    /// <seealso cref="IPersistedGrantDbContext" />
    public sealed class ConfigurationDbContext : IConfigurationDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly ConfigurationStoreOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="ConfigurationDbContext"/>.
        /// </summary>
        /// <param name="client">MongoDB client</param>
        /// <param name="options">Configuration store options</param>
        public ConfigurationDbContext(IMongoClient client, ConfigurationStoreOptions options)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.DatabaseName))
                throw new ArgumentException($"DatabaseName is missing in {nameof(ConfigurationStoreOptions)}");

            _db = client.GetDatabase(options.DatabaseName);
            _options = options;
        }

        public IMongoCollection<Client> Clients => _db.GetCollection<Client>(_options.ClientsCollectionName);

        public IMongoCollection<ClientCorsOrigin> ClientCorsOrigins => _db.GetCollection<ClientCorsOrigin>(_options.ClientCorsOriginsCollectionName);

        public IMongoCollection<IdentityResource> IdentityResources => _db.GetCollection<IdentityResource>(_options.IdentityResourcesCollectionName);

        public IMongoCollection<ApiResource> ApiResources => _db.GetCollection<ApiResource>(_options.ApiResourcesCollectionName);

        public IMongoCollection<ApiScope> ApiScopes => _db.GetCollection<ApiScope>(_options.ApiScopesCollectionName);

        public IMongoCollection<IdentityProvider> IdentityProviders => _db.GetCollection<IdentityProvider>(_options.IdentityProvidersCollectionName);
    }
}
