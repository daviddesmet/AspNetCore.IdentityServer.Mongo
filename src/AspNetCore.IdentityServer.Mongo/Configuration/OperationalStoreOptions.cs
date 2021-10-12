namespace AspNetCore.IdentityServer.Mongo.Configuration
{
    /// <summary>
    /// Options for configuring the operational context.
    /// </summary>
    public sealed class OperationalStoreOptions
    {
        /// <summary>
        /// Gets the Database Name.
        /// </summary>
        public string? DatabaseName { get; set; }

        /// <summary>
        /// Gets or sets the name of the persisted grants collection.
        /// </summary>
        /// <value>The name of the persisted grants collection.</value>
        public string PersistedGrantsCollectionName { get; set; } = "PersistedGrants";

        /// <summary>
        /// Gets or sets the name of the device flow codes collection.
        /// </summary>
        /// <value>The name of the device flow codes collection.</value>
        public string DeviceFlowCodesCollectionName { get; set; } = "DeviceFlowCodes";

        /// <summary>
        /// Gets or sets the name of the keys collection.
        /// </summary>
        /// <value>The name of the keys collection.</value>
        public string KeysCollectionName { get; set; } = "Keys";

        /// <summary>
        /// Gets or sets the name of the jobs collection.
        /// </summary>
        /// <value>The name of the jobs collection.</value>
        public string JobsCollectionName { get; set; } = "Jobs";

        /// <summary>
        /// Gets or sets a value indicating whether stale entries will be automatically cleaned up from the database.
        /// This is implemented by periodically connecting to the database (according to the TokenCleanupInterval) from the hosting application.
        /// Defaults to false.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable token cleanup]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTokenCleanup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether consumed tokens will included in the automatic clean up.
        /// </summary>
        /// <value>
        ///   <c>true</c> if consumed tokens are to be included in cleanup; otherwise, <c>false</c>.
        /// </value>
        public bool RemoveConsumedTokens { get; set; }

        /// <summary>
        /// Gets or sets the token cleanup interval (in seconds). The default is 3600 (1 hour).
        /// </summary>
        /// <value>
        /// The token cleanup interval.
        /// </value>
        public int TokenCleanupInterval { get; set; } = 3600;

        /// <summary>
        /// Gets or sets the number of records to remove at a time. Defaults to 100.
        /// </summary>
        /// <value>
        /// The size of the token cleanup batch.
        /// </value>
        public int TokenCleanupBatchSize { get; set; } = 100;

        /// <summary>
        /// Gets or sets the worker expiration in seconds. Defaults to 600 (10 minutes).
        /// This is implemented so when there are several replicas of the service, only one service is asigned a cleanup task.
        /// </summary>
        /// <value>
        /// The worker expiration.
        /// </value>
        public int WorkerExpiration { get; set; } = 600;
    }
}
