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

    using AspNetCore.IdentityServer.Mongo.Interfaces;
    using AspNetCore.IdentityServer.Mongo.Mappers;

    /// <summary>
    /// Implementation of IIdentityProviderStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="IClientStore" />
    public class IdentityProviderStore : IIdentityProviderStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityProviderStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public IdentityProviderStore(IConfigurationDbContext context, ILogger<IdentityProviderStore> logger)
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
        protected ILogger<IdentityProviderStore> Logger { get; private set; }

        /// <inheritdoc/>
        public async Task<IEnumerable<IdentityProviderName>> GetAllSchemeNamesAsync()
        {
            var query = Context.IdentityProviders.AsQueryable().Select(x => new IdentityProviderName
            {
                Enabled = x.Enabled,
                Scheme = x.Scheme,
                DisplayName = x.DisplayName
            });

            return await query.ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IdentityProvider?> GetBySchemeAsync(string scheme)
        {
            var idp = await Context.IdentityProviders.AsQueryable().Where(x => x.Scheme == scheme).SingleOrDefaultAsync();
            if (idp is null)
                return null;

            var result = MapIdp(idp);
            if (result is null)
                Logger.LogError("Identity provider record found in database, but mapping failed for scheme {Scheme} and protocol type {Protocol}", idp.Scheme, idp.Type);

            return result;
        }

        /// <summary>
        /// Maps from the identity provider entity to identity provider model.
        /// </summary>
        /// <param name="idp"></param>
        /// <returns></returns>
        protected virtual IdentityProvider? MapIdp(Entities.IdentityProvider idp)
        {
            if (idp.Type == "oidc")
                return new OidcProvider(idp.ToModel());

            return null;
        }
    }
}
