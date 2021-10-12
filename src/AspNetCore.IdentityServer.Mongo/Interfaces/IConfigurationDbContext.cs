namespace AspNetCore.IdentityServer.Mongo.Interfaces
{
    using AspNetCore.IdentityServer.Mongo.Entities;
    using MongoDB.Driver;

    public interface IConfigurationDbContext
    {
        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        /// <value>
        /// The clients.
        /// </value>
        IMongoCollection<Client> Clients { get; }

        /// <summary>
        /// Gets or sets the clients' CORS origins.
        /// </summary>
        /// <value>
        /// The clients CORS origins.
        /// </value>
        IMongoCollection<ClientCorsOrigin> ClientCorsOrigins { get; }

        /// <summary>
        /// Gets or sets the identity resources.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        IMongoCollection<IdentityResource> IdentityResources { get; }

        /// <summary>
        /// Gets or sets the API resources.
        /// </summary>
        /// <value>
        /// The API resources.
        /// </value>
        IMongoCollection<ApiResource> ApiResources { get; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The identity resources.
        /// </value>
        IMongoCollection<ApiScope> ApiScopes { get; }

        /// <summary>
        /// Gets or sets the identity providers.
        /// </summary>
        /// <value>
        /// The identity providers.
        /// </value>
        IMongoCollection<IdentityProvider> IdentityProviders { get; }
    }
}
