namespace AspNetCore.IdentityServer.Mongo.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics;
    using MongoDB.Bson.Serialization.Attributes;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class ApiResourceScope
    {
        private string DebuggerDisplay => Scope ?? $"{{{typeof(ApiResourceScope)}}}";

        [BsonRequired]
        [MaxLength(200)]
        public string Scope { get; set; } = default!;
    }
}
