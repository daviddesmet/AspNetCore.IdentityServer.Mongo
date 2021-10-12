namespace AspNetCore.IdentityServer.Mongo.Stores
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Stores;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Interfaces;
    using AspNetCore.IdentityServer.Mongo.Mappers;

    /// <summary>
    /// Implementation of IResourceStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="IResourceStore" />
    public class ResourceStore : IResourceStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public ResourceStore(IConfigurationDbContext context, ILogger<ResourceStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The DbContext.
        /// </summary>
        protected IConfigurationDbContext Context { get; private set; }

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger<ResourceStore> Logger { get; private set; }

        /// <summary>
        /// Finds the API resources by name.
        /// </summary>
        /// <param name="apiResourceNames">The names.</param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Duende.IdentityServer.Models.ApiResource?>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            if (apiResourceNames is null)
                throw new ArgumentNullException(nameof(apiResourceNames));

            var query = await Context.ApiResources.FindAsync(x => apiResourceNames.Contains(x.Name));
            var results = await query.ToListAsync();

            var models = results.Select(x => x.ToModel());

            if (models.Any())
                Logger.LogDebug("Found {Apis} API resource in database", results.Select(x => x.Name));
            else
                Logger.LogDebug("Did not find {Apis} API resource in database", apiResourceNames);

            return models;
        }

        /// <summary>
        /// Gets API resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Duende.IdentityServer.Models.ApiResource?>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var query = await Context.ApiResources.FindAsync(x => x.Scopes.Any(s => names.Contains(s.Scope)));
            var results = await query.ToListAsync();

            var models = results.Select(x => x.ToModel());

            Logger.LogDebug("Found {Apis} API resources in database", models.Select(x => x!.Name));

            return models;
        }

        /// <summary>
        /// Gets identity resources by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Duende.IdentityServer.Models.IdentityResource?>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query = await Context.IdentityResources.FindAsync(x => scopes.Contains(x.Name));
            var results = await query.ToListAsync();

            Logger.LogDebug("Found {IdentityResources} identity scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        /// <summary>
        /// Gets scopes by scope name.
        /// </summary>
        /// <param name="scopeNames"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Duende.IdentityServer.Models.ApiScope?>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query = await Context.ApiScopes.FindAsync(x => scopes.Contains(x.Name));
            var results = await query.ToListAsync();

            Logger.LogDebug("Found {Scopes} scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        /// <summary>
        /// Gets all resources.
        /// </summary>
        /// <returns></returns>
        public virtual async Task<Duende.IdentityServer.Models.Resources> GetAllResourcesAsync()
        {
            var identity = await (await Context.IdentityResources.FindAsync(_ => true)).ToListAsync();
            var apis = await (await Context.ApiResources.FindAsync(_ => true)).ToListAsync();
            var scopes = await (await Context.ApiScopes.FindAsync(_ => true)).ToListAsync();

            var result = new Duende.IdentityServer.Models.Resources(
                identity.Select(x => x.ToModel()),
                apis.Select(x => x.ToModel()),
                scopes.Select(x => x.ToModel())
            );

            Logger.LogDebug("Found {Scopes} as all scopes, and {Apis} as API resources",
                result.IdentityResources.Select(x => x.Name).Union(result.ApiScopes.Select(x => x.Name)),
                result.ApiResources.Select(x => x.Name));

            return result;
        }
    }
}
