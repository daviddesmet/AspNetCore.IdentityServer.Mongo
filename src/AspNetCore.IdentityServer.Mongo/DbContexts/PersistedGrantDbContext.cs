namespace AspNetCore.IdentityServer.Mongo.DbContexts
{
    using System;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// DbContext for the IdentityServer operational data.
    /// </summary>
    /// <seealso cref="IPersistedGrantDbContext" />
    public sealed class PersistedGrantDbContext : IPersistedGrantDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly OperationalStoreOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="PersistedGrantDbContext"/>.
        /// </summary>
        /// <param name="client">MongoDB client</param>
        /// <param name="options">Operational store options</param>
        public PersistedGrantDbContext(IMongoClient client, OperationalStoreOptions options)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.DatabaseName))
                throw new ArgumentException($"DatabaseName is missing in {nameof(OperationalStoreOptions)}");

            _db = client.GetDatabase(options.DatabaseName);
            _options = options;
        }

        public IMongoCollection<PersistedGrant> PersistedGrants => _db.GetCollection<PersistedGrant>(_options.PersistedGrantsCollectionName);

        public IMongoCollection<DeviceFlowCode> DeviceFlowCodes => _db.GetCollection<DeviceFlowCode>(_options.DeviceFlowCodesCollectionName);

        public IMongoCollection<Key> Keys => _db.GetCollection<Key>(_options.KeysCollectionName);
    }
}
