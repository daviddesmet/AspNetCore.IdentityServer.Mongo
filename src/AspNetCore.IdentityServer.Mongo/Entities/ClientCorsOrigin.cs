namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ClientCorsOrigin
    {
        private string DebuggerDisplay => Origin ?? $"{{{typeof(ClientCorsOrigin)}}}";

        [BsonRequired]
        [MaxLength(150)]
        public string Origin { get; set; } = default!;
    }
}
