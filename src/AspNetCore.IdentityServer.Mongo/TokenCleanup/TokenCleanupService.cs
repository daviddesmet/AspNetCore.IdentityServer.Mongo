namespace AspNetCore.IdentityServer.Mongo.TokenCleanup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;
    using MongoDB.Driver;
    using MongoDB.Driver.Linq;

    using AspNetCore.IdentityServer.Mongo.Configuration;
    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Entities.Jobs;
    using AspNetCore.IdentityServer.Mongo.Interfaces;
    using AspNetCore.IdentityServer.Mongo.Stores;

    /// <summary>
    /// Helper to cleanup stale persisted grants and device codes.
    /// </summary>
    public class TokenCleanupService
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly IJobSchedulerDbContext _scheduler;
        private readonly OperationalStoreOptions _options;
        private readonly ILogger<TokenCleanupService> _logger;
        private readonly IOperationalStoreNotification? _operationalStoreNotification;

        internal const string REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID = "RemoveExpiredPersistedGrants";
        internal const string REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID = "RemoveConsumedPersistedGrants";
        internal const string REMOVE_DEVICECODES_TASKID = "RemoveDeviceCodes";

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="scheduler">The job scheduler context.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="operationalStoreNotification">The store notification.</param>
        public TokenCleanupService(IPersistedGrantDbContext context, IJobSchedulerDbContext scheduler, OperationalStoreOptions options, ILogger<TokenCleanupService> logger, IOperationalStoreNotification? operationalStoreNotification = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _scheduler = scheduler ?? throw new ArgumentNullException(nameof(scheduler));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _operationalStoreNotification = operationalStoreNotification;
        }

        /// <summary>
        /// Method to clear expired persisted grants.
        /// </summary>
        /// <returns></returns>
        public async Task RemoveExpiredGrantsAsync()
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync();
                await RemoveDeviceCodesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception removing expired grants: {exception}", ex.Message);
            }
        }

        /// <summary>
        /// Removes the stale persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveGrantsAsync()
        {
            await RemoveExpiredPersistedGrantsAsync();
            if (_options.RemoveConsumedTokens)
                await RemoveConsumedPersistedGrantsAsync();
        }

        /// <summary>
        /// Removes the expired persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveExpiredPersistedGrantsAsync()
        {
            var found = int.MaxValue;

            if (!await IsJobReserved(REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID, Dns.GetHostName()))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredGrants = await _context.PersistedGrants.AsQueryable().Where(x => x.Expiration < DateTime.UtcNow)
                                                                                .OrderBy(x => x.Expiration)
                                                                                .Select(x => x.Key)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync();

                found = expiredGrants.Count;

                _logger.LogInformation("Removing {GrantCount} expired grants", found);

                if (found > 0)
                {
                    try
                    {
                        await RemovePersistedGrantsAsync(expiredGrants);

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing expired grants: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservation(REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID, Dns.GetHostName());
        }

        /// <summary>
        /// Removes the consumed persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveConsumedPersistedGrantsAsync()
        {
            var found = int.MaxValue;

            if (!await IsJobReserved(REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID, Dns.GetHostName()))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredGrants = await _context.PersistedGrants.AsQueryable().Where(x => x.ConsumedTime < DateTime.UtcNow)
                                                                                .OrderBy(x => x.ConsumedTime)
                                                                                .Select(x => x.Key)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync();

                found = expiredGrants.Count;

                _logger.LogInformation("Removing {GrantCount} consumed grants", found);

                if (found > 0)
                {
                    try
                    {
                        await RemovePersistedGrantsAsync(expiredGrants);

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing consumed grants: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservation(REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID, Dns.GetHostName());
        }

        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync()
        {
            var found = int.MaxValue;

            if (!await IsJobReserved(REMOVE_DEVICECODES_TASKID, Dns.GetHostName()))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredCodes = await _context.DeviceFlowCodes.AsQueryable().Where(x => x.Expiration < DateTime.UtcNow)
                                                                                .OrderBy(x => x.DeviceCode)
                                                                                .Select(x => x.DeviceCode)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync();

                found = expiredCodes.Count;

                _logger.LogInformation("Removing {DeviceCodeCount} device flow codes", found);

                if (found > 0)
                {
                    try
                    {
                        var requests = new List<WriteModel<DeviceFlowCode>>();
                        var filter = Builders<DeviceFlowCode>.Filter.In(x => x.DeviceCode, expiredCodes);
                        requests.Add(new DeleteManyModel<DeviceFlowCode>(filter));
                        await _context.DeviceFlowCodes.BulkWriteAsync(requests, new BulkWriteOptions { BypassDocumentValidation = true, IsOrdered = true });

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredCodes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing expired device codes: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservation(REMOVE_DEVICECODES_TASKID, Dns.GetHostName());
        }

        private async Task RemovePersistedGrantsAsync(IList<string> grants)
        {
            var requests = new List<WriteModel<PersistedGrant>>();
            var filter = Builders<PersistedGrant>.Filter.In(x => x.Key, grants);
            requests.Add(new DeleteManyModel<PersistedGrant>(filter));
            await _context.PersistedGrants.BulkWriteAsync(requests, new BulkWriteOptions { BypassDocumentValidation = true, IsOrdered = true });
        }

        private async Task<bool> IsJobReserved(string taskId, string workerId)
        {
            var filter = Builders<JobSchedule>.Filter.Eq(x => x.TaskId, taskId)
                & (Builders<JobSchedule>.Filter.Eq(x => x.WorkerId, null) & Builders<JobSchedule>.Filter.Eq(x => x.WorkerExpiration, null))
                | (Builders<JobSchedule>.Filter.Lt(x => x.WorkerExpiration, DateTime.UtcNow));
            var update = Builders<JobSchedule>.Update.Set(x => x.WorkerId, workerId).Set(x => x.WorkerExpiration, DateTime.UtcNow.AddSeconds(_options.WorkerExpiration));
            var result = await _scheduler.Jobs.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<JobSchedule> { IsUpsert = false, ReturnDocument = ReturnDocument.After });
            var reserved = result?.WorkerId == workerId;

            _logger.LogDebug("Reserving Job {TaskId} to {WorkerId}: {Reserved}", taskId, workerId, reserved);

            return reserved;
        }

        private async Task ClearJobReservation(string taskId, string workerId)
        {
            _logger.LogDebug("Cleaning Job Reservation for {TaskId}", taskId);

            var filter = Builders<JobSchedule>.Filter.Eq(x => x.TaskId, taskId) & Builders<JobSchedule>.Filter.Eq(x => x.WorkerId, workerId);
            var update = Builders<JobSchedule>.Update.Set(x => x.WorkerId, null).Set(x => x.WorkerExpiration, null);
            await _scheduler.Jobs.UpdateOneAsync(filter, update);
        }
    }
}
