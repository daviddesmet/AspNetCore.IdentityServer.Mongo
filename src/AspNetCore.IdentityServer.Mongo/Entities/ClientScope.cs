namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientScope
    {
        private string DebuggerDisplay => Scope ?? $"{{{typeof(ClientScope)}}}";

        [BsonRequired]
        [MaxLength(200)]
        public string Scope { get; set; } = default!;
    }
}
