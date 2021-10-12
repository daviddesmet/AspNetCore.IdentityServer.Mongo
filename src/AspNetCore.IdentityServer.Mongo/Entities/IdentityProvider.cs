namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;

    /// <summary>
    /// Models storage for identity providers.
    /// </summary>
    public class IdentityProvider
    {
        /// <summary>
        /// Scheme name for the provider.
        /// </summary>
        [BsonId]
        [MaxLength(200)]
        public string Scheme { get; set; } = default!;

        /// <summary>
        /// Display name for the provider.
        /// </summary>
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Flag that indicates if the provider should be used.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Protocol type of the provider.
        /// </summary>
        [MaxLength(20)]
        public string Type { get; set; } = default!;

        /// <summary>
        /// Seralized value for the identity provider properties dictionary.
        /// </summary>
        public string? Properties { get; set; }
    }
}
