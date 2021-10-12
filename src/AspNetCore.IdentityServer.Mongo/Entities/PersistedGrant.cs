namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Models storage for persistent grants.
    /// </summary>
    public class PersistedGrant
    {
        /// <summary>
        /// Gets or sets the grant key.
        /// </summary>
        /// <value>
        /// The grant key.
        /// </value>
        [BsonId]
        [StringLength(200)]
        public string Key { get; set; } = default!;

        /// <summary>
        /// Gets or sets the grant type.
        /// </summary>
        /// <value>
        /// The grant type.
        /// </value>
        [BsonRequired]
        [StringLength(50)]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        [StringLength(200)]
        public string? SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [StringLength(100)]
        public string? SessionId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [BsonRequired]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [StringLength(200)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        [BsonRequired]
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration.
        /// </value>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the consumed time.
        /// </summary>
        /// <value>
        /// The consumed time.
        /// </value>
        public DateTime? ConsumedTime { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        [BsonRequired]
        [MaxLength(50000)]
        public string Data { get; set; } = default!;
    }
}
