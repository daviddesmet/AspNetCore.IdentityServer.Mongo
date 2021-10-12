namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientIdPRestriction
    {
        private string DebuggerDisplay => Provider ?? $"{{{typeof(ClientIdPRestriction)}}}";

        [BsonRequired]
        [MaxLength(200)]
        public string Provider { get; set; } = default!;
    }
}
