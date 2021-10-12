namespace AspNetCore.IdentityServer.Mongo.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Models;
    using Duende.IdentityServer.Stores;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// Implementation of ISigningKeyStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="ISigningKeyStore" />
    public class SigningKeyStore : ISigningKeyStore
    {
        private const string Use = "signing";

        /// <summary>
        /// Initializes a new instance of the <see cref="SigningKeyStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public SigningKeyStore(IPersistedGrantDbContext context, ILogger<SigningKeyStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The DbContext.
        /// </summary>One
        protected IPersistedGrantDbContext Context { get; private set; }

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger<SigningKeyStore> Logger { get; private set; }

        /// <inheritdoc/>
        public async Task<IEnumerable<SerializedKey>> LoadKeysAsync()
        {
            var keys = await Context.Keys.AsQueryable().Where(x => x.Use == Use).ToListAsync();

            if (keys.Any())
                Logger.LogDebug("Found {Count} signing keys in database", keys.Count);

            return keys.Select(key => new SerializedKey
            {
                Id = key.Id,
                Created = key.Created,
                Version = key.Version,
                Algorithm = key.Algorithm,
                Data = key.Data,
                DataProtected = key.DataProtected,
                IsX509Certificate = key.IsX509Certificate
            });
        }

        /// <inheritdoc/>
        public async Task StoreKeyAsync(SerializedKey key)
        {
            var entity = new Key
            {
                Id = key.Id,
                Use = Use,
                Created = key.Created,
                Version = key.Version,
                Algorithm = key.Algorithm,
                Data = key.Data,
                DataProtected = key.DataProtected,
                IsX509Certificate = key.IsX509Certificate
            };
            await Context.Keys.InsertOneAsync(entity);
        }

        /// <inheritdoc/>
        public async Task DeleteKeyAsync(string id)
        {
            var result = await Context.Keys.DeleteOneAsync(x => x.Use == Use && x.Id == id);
            if (result.DeletedCount == 0)
            {
                Logger.LogDebug("No {KeyId} key id found in database", id);
                return;
            }

            Logger.LogDebug("Removed {KeyId} key id from database", id);
        }
    }
}
