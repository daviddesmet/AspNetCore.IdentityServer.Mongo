namespace AspNetCore.IdentityServer.Mongo.DbContexts
{
    using System;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities.Jobs;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// DbContext for the Job scheduler data.
    /// </summary>
    /// <seealso cref="IJobSchedulerDbContext" />
    public sealed class JobSchedulerDbContext : IJobSchedulerDbContext
    {
        private readonly IMongoDatabase _db;
        private readonly OperationalStoreOptions _options;

        /// <summary>
        /// Creates a new instance of the <see cref="JobSchedulerDbContext"/>.
        /// </summary>
        /// <param name="client">MongoDB client</param>
        /// <param name="options">Operational store options</param>
        public JobSchedulerDbContext(IMongoClient client, OperationalStoreOptions options)
        {
            if (client is null)
                throw new ArgumentNullException(nameof(client));

            if (options is null)
                throw new ArgumentNullException(nameof(options));

            if (string.IsNullOrWhiteSpace(options.DatabaseName))
                throw new ArgumentException($"DatabaseName is missing in {nameof(OperationalStoreOptions)}");

            _db = client.GetDatabase(options.DatabaseName);
            _options = options;
        }

        public IMongoCollection<JobSchedule> Jobs => _db.GetCollection<JobSchedule>(_options.JobsCollectionName);
    }
}
