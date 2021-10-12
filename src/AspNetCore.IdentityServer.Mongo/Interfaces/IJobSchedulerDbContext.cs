namespace AspNetCore.IdentityServer.Mongo.Interfaces
{
    using AspNetCore.IdentityServer.Mongo.Entities.Jobs;
    using MongoDB.Driver;

    /// <summary>
    /// Abstraction for the job scheduler data context.
    /// </summary>
    public interface IJobSchedulerDbContext
    {
        /// <summary>
        /// Gets or sets the job schedule.
        /// </summary>
        /// <value>
        /// The job schedule.
        /// </value>
        IMongoCollection<JobSchedule> Jobs { get; }
    }
}
