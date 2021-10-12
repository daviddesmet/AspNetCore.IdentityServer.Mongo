namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientClaim
    {
        private string DebuggerDisplay => Type ?? $"{{{typeof(ClientClaim)}}}";

        [BsonRequired]
        [MaxLength(250)]
        public string Type { get; set; } = default!;

        [BsonRequired]
        [MaxLength(250)]
        public string Value { get; set; } = default!;
    }
}
