namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using MongoDB.Bson.Serialization.Attributes;
    using static Duende.IdentityServer.IdentityServerConstants;

    public abstract class Secret
    {
        [MaxLength(2000)]
        public string? Description { get; set; }

        [BsonRequired]
        [MaxLength(4000)]
        public string Value { get; set; } = default!;

        [BsonRequired]
        [MaxLength(250)]
        public string Type { get; set; } = SecretTypes.SharedSecret;

        public DateTime? Expiration { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
