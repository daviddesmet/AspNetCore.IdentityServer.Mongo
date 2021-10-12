namespace AspNetCore.IdentityServer.Mongo.Entities.Jobs
{
    using System;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class JobSchedule
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string TaskId { get; set; } = default!;

        public string? WorkerId { get; set; }

        public DateTime? WorkerExpiration { get; set; }
    }
}
