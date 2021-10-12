namespace AspNetCore.IdentityServer.Mongo.Interfaces
{
    using AspNetCore.IdentityServer.Mongo.Entities;
    using MongoDB.Driver;

    /// <summary>
    /// Abstraction for the operational data context.
    /// </summary>
    public interface IPersistedGrantDbContext
    {
        /// <summary>
        /// Gets or sets the persisted grants.
        /// </summary>
        /// <value>
        /// The persisted grants.
        /// </value>
        IMongoCollection<PersistedGrant> PersistedGrants { get; }

        /// <summary>
        /// Gets or sets the device flow codes.
        /// </summary>
        /// <value>
        /// The device flow codes.
        /// </value>
        IMongoCollection<DeviceFlowCode> DeviceFlowCodes { get; }

        /// <summary>
        /// Gets or sets the keys.
        /// </summary>
        /// <value>
        /// The keys.
        /// </value>
        IMongoCollection<Key> Keys { get; }
    }
}
