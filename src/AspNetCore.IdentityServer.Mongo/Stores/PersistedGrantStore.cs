namespace AspNetCore.IdentityServer.Mongo.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Extensions;
    using Duende.IdentityServer.Stores;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Interfaces;
    using AspNetCore.IdentityServer.Mongo.Mappers;

    /// <summary>
    /// Implementation of IPersistedGrantStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="IPersistedGrantStore" />
    public class PersistedGrantStore : IPersistedGrantStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public PersistedGrantStore(IPersistedGrantDbContext context, ILogger<PersistedGrantStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The DbContext.
        /// </summary>
        protected IPersistedGrantDbContext Context { get; private set; }

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger<PersistedGrantStore> Logger { get; private set; }

        /// <inheritdoc/>
        public virtual async Task StoreAsync(Duende.IdentityServer.Models.PersistedGrant grant)
        {
            var result = await Context.PersistedGrants!.ReplaceOneAsync(x => x!.Key == grant.Key, grant.ToEntity(), new ReplaceOptions { IsUpsert = true });
            if (result.MatchedCount == 0)
            {
                Logger.LogDebug("{PersistedGrantKey} persisted grant not found in database", grant.Key);
                Logger.LogDebug("Created {PersistedGrantKey} persisted grant in database", grant.Key);
                return;
            }

            Logger.LogDebug("{PersistedGrantKey} persisted grant found in database", grant.Key);
            Logger.LogDebug("Updated {PersistedGrantKey} persisted grant in database", grant.Key);
        }

        /// <inheritdoc/>
        public virtual async Task<Duende.IdentityServer.Models.PersistedGrant?> GetAsync(string key)
        {
            var persistedGrant = await Context.PersistedGrants.AsQueryable().Where(x => x.Key == key).SingleOrDefaultAsync();
            var model = persistedGrant?.ToModel();

            Logger.LogDebug("{PersistedGrantKey} persisted grant found in database: {PersistedGrantKeyFound}", key, model != null);

            return model;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Duende.IdentityServer.Models.PersistedGrant?>> GetAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            var persistedGrants = await Filter(Context.PersistedGrants.AsQueryable(), filter).ToListAsync();

            var model = persistedGrants.Select(x => x.ToModel());

            Logger.LogDebug("{PersistedGrantCount} persisted grants found using {@Filter}", persistedGrants.Count, filter);

            return model;
        }

        /// <inheritdoc/>
        public virtual async Task RemoveAsync(string key)
        {
            Logger.LogDebug("Removing {PersistedGrantKey} persisted grant from database", key);

            var result = await Context.PersistedGrants.DeleteOneAsync(x => x.Key == key);
            if (result.DeletedCount == 0)
            {
                Logger.LogDebug("No {PersistedGrantKey} persisted grant found in database", key);
                return;
            }

            Logger.LogDebug("Removed {PersistedGrantKey} persisted grant from database", key);
        }

        /// <inheritdoc/>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            filter.Validate();

            Logger.LogDebug("Removing persisted grants from database using {@filter}", filter);

            var result = await Context.PersistedGrants.DeleteManyAsync(x => (string.IsNullOrWhiteSpace(filter.ClientId) || x.SubjectId == filter.ClientId) &&
                                                                            (string.IsNullOrWhiteSpace(filter.SessionId) || x.SubjectId == filter.SessionId) &&
                                                                            (string.IsNullOrWhiteSpace(filter.SubjectId) || x.ClientId == filter.SubjectId) &&
                                                                            (string.IsNullOrWhiteSpace(filter.Type) || x.Type == filter.Type));

            Logger.LogDebug("Removed {PersistedGrantCount} persisted grants from database for {@Filter}", result.DeletedCount, filter);
        }

        private IMongoQueryable<PersistedGrant> Filter(IMongoQueryable<PersistedGrant> query, PersistedGrantFilter filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.ClientId))
                query = query.Where(x => x.ClientId == filter.ClientId);

            if (!string.IsNullOrWhiteSpace(filter.SessionId))
                query = query.Where(x => x.SessionId == filter.SessionId);

            if (!string.IsNullOrWhiteSpace(filter.SubjectId))
                query = query.Where(x => x.SubjectId == filter.SubjectId);

            if (!string.IsNullOrWhiteSpace(filter.Type))
                query = query.Where(x => x.Type == filter.Type);

            return query;
        }
    }
}
