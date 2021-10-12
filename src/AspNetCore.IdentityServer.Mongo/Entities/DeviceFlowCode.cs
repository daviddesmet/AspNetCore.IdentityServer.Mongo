namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Entity for device flow codes
    /// </summary>
    public class DeviceFlowCode
    {
        /// <summary>
        /// Gets or sets the device code.
        /// </summary>
        /// <value>
        /// The device code.
        /// </value>
        [BsonRequired]
        [MaxLength(200)]
        public string DeviceCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        /// <value>
        /// The user code.
        /// </value>
        [BsonId]
        [MaxLength(200)]
        public string UserCode { get; set; } = default!;

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        [MaxLength(200)]
        public string? SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        [MaxLength(100)]
        public string? SessionId { get; set; }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        [BsonRequired]
        [MaxLength(200)]
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [MaxLength(200)]
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
        [BsonRequired]
        public DateTime? Expiration { get; set; }

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
