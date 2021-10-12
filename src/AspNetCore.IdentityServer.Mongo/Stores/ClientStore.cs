namespace AspNetCore.IdentityServer.Mongo.Stores
{
    using System;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Stores;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using AspNetCore.IdentityServer.Mongo.Interfaces;
    using AspNetCore.IdentityServer.Mongo.Mappers;

    /// <summary>
    /// Implementation of IClientStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="IClientStore" /
    public class ClientStore : IClientStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public ClientStore(IConfigurationDbContext context, ILogger<ClientStore> logger)
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
        protected ILogger<ClientStore> Logger { get; private set; }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public virtual async Task<Duende.IdentityServer.Models.Client?> FindClientByIdAsync(string clientId)
        {
            var client = await Context.Clients.AsQueryable().Where(x => x.ClientId == clientId).SingleOrDefaultAsync();
            var model = client?.ToModel();

            Logger.LogDebug("{ClientId} found in database: {ClientIdFound}", clientId, model != null);

            return model;
        }
    }
}
