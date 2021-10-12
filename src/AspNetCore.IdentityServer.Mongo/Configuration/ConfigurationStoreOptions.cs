namespace AspNetCore.IdentityServer.Mongo.Configuration
{
    /// <summary>
    /// Options for configuring the configuration context.
    /// </summary>
    public sealed class ConfigurationStoreOptions
    {
        /// <summary>
        /// Gets the Database Name.
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the clients collection.
        /// </summary>
        /// <value>The name of the clients collection.</value>
        public string ClientsCollectionName { get; set; } = "Clients";

        /// <summary>
        /// Gets or sets the name of the client cors origins collection.
        /// </summary>
        /// <value>The name of the client cors origins collection.</value>
        public string ClientCorsOriginsCollectionName { get; set; } = "ClientCorsOrigins";

        /// <summary>
        /// Gets or sets the name of the identity resources collection.
        /// </summary>
        /// <value>The name of the identity resources collection.</value>
        public string IdentityResourcesCollectionName { get; set; } = "IdentityResources";

        /// <summary>
        /// Gets or sets the name of the API resources collection.
        /// </summary>
        /// <value>The name of the API resources collection.</value>
        public string ApiResourcesCollectionName { get; set; } = "ApiResources";

        /// <summary>
        /// Gets or sets the name of the API scopes collection.
        /// </summary>
        /// <value>The name of the API scopes collection.</value>
        public string ApiScopesCollectionName { get; set; } = "ApiScopes";

        /// <summary>
        /// Gets or sets the name of the identity providers collection.
        /// </summary>
        /// <value>The name of the identity providers collection.</value>
        public string IdentityProvidersCollectionName { get; set; } = "IdentityProviders";
    }
}
