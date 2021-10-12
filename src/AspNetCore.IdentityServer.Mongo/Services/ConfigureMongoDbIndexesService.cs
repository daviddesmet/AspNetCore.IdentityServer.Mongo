namespace AspNetCore.IdentityServer.Mongo.Services
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities;

    public class ConfigureMongoDbIndexesService : IHostedService
    {
        private readonly IMongoClient _client;
        private readonly OperationalStoreOptions _options;
        private readonly ILogger<ConfigureMongoDbIndexesService> _logger;

        public ConfigureMongoDbIndexesService(IMongoClient client, OperationalStoreOptions options, ILogger<ConfigureMongoDbIndexesService> logger) => (_client, _options, _logger) = (client, options, logger);

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var database = _client.GetDatabase(_options.DatabaseName);

            _logger.LogInformation("Creating indexes on DeviceFlowCodes");
            var deviceFlowCodesCollection = database.GetCollection<DeviceFlowCode>(_options.DeviceFlowCodesCollectionName);
            await EnsureOneIndexAsync(deviceFlowCodesCollection, x => x.DeviceCode, unique: true, cancellationToken);
            await EnsureOneIndexAsync(deviceFlowCodesCollection, x => x.Expiration!, cancellationToken: cancellationToken);

            _logger.LogInformation("Creating indexes on PersistedGrants");
            var persistedGrantsCollection = database.GetCollection<PersistedGrant>(_options.PersistedGrantsCollectionName);
            await EnsureOneIndexAsync(persistedGrantsCollection, x => x.Expiration!, cancellationToken: cancellationToken);
            await EnsureOneIndexAsync(persistedGrantsCollection, x => x.ConsumedTime!, cancellationToken: cancellationToken);
            await EnsureCombinedIndexAsync(persistedGrantsCollection, cancellationToken, x => x.SubjectId!, x => x.ClientId, x => x.Type);
            await EnsureCombinedIndexAsync(persistedGrantsCollection, cancellationToken, x => x.SubjectId!, x => x.SessionId!, x => x.Type);

            _logger.LogInformation("Creating 'Use' index on Keys");
            var keysCollection = database.GetCollection<Key>(_options.KeysCollectionName);
            await EnsureOneIndexAsync(keysCollection, x => x.Use, cancellationToken: cancellationToken);
        }


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task EnsureOneIndexAsync<TEntity>(IMongoCollection<TEntity> collection, Expression<Func<TEntity, object>> field, bool unique = false, CancellationToken cancellationToken = default)
        {
            var options = new CreateIndexOptions() { Background = true, Unique = unique };
            var model = new CreateIndexModel<TEntity>(Builders<TEntity>.IndexKeys.Ascending(field), options);
            await collection.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }

        private static async Task EnsureCombinedIndexAsync<TEntity>(IMongoCollection<TEntity> collection, CancellationToken cancellationToken = default, params Expression<Func<TEntity, object>>[] fields)
        {
            var options = new CreateIndexOptions() { Background = true };
            var idxBuilder = Builders<TEntity>.IndexKeys;

            var keys = fields.AsEnumerable().Select(field => idxBuilder.Ascending(field)).ToArray();
            var model = new CreateIndexModel<TEntity>(idxBuilder.Combine(keys), options);
            await collection.Indexes.CreateOneAsync(model, cancellationToken: cancellationToken);
        }
    }
}
