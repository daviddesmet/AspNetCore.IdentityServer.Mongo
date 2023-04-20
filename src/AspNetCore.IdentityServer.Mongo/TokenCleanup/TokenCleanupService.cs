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
    using System.Threading;

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
        public async Task RemoveExpiredGrantsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogTrace("Querying for expired grants to remove");

                await RemoveGrantsAsync(cancellationToken);
                await RemoveDeviceCodesAsync(cancellationToken);
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
        protected virtual async Task RemoveGrantsAsync(CancellationToken cancellationToken = default)
        {
            await RemoveExpiredPersistedGrantsAsync(cancellationToken);
            if (_options.RemoveConsumedTokens)
                await RemoveConsumedPersistedGrantsAsync(cancellationToken);
        }

        /// <summary>
        /// Removes the expired persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveExpiredPersistedGrantsAsync(CancellationToken cancellationToken = default)
        {
            var found = int.MaxValue;

            if (!await IsJobReservedAsync(REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID, Dns.GetHostName(), cancellationToken))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredGrants = await _context.PersistedGrants.AsQueryable().Where(x => x.Expiration < DateTime.UtcNow)
                                                                                .OrderBy(x => x.Expiration)
                                                                                .Select(x => x.Key)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync(cancellationToken);

                found = expiredGrants.Count;

                _logger.LogInformation("Removing {GrantCount} expired grants", found);

                if (found > 0)
                {
                    try
                    {
                        await RemovePersistedGrantsAsync(expiredGrants, cancellationToken);

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing expired grants: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservationAsync(REMOVE_EXPIRED_PERSISTEDGRANTS_TASKID, Dns.GetHostName());
        }

        /// <summary>
        /// Removes the consumed persisted grants.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveConsumedPersistedGrantsAsync(CancellationToken cancellationToken = default)
        {
            var found = int.MaxValue;

            if (!await IsJobReservedAsync(REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID, Dns.GetHostName(), cancellationToken))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredGrants = await _context.PersistedGrants.AsQueryable().Where(x => x.ConsumedTime < DateTime.UtcNow)
                                                                                .OrderBy(x => x.ConsumedTime)
                                                                                .Select(x => x.Key)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync(cancellationToken);

                found = expiredGrants.Count;

                _logger.LogInformation("Removing {GrantCount} consumed grants", found);

                if (found > 0)
                {
                    try
                    {
                        await RemovePersistedGrantsAsync(expiredGrants, cancellationToken);

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredGrants, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing consumed grants: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservationAsync(REMOVE_CONSUMED_PERSISTEDGRANTS_TASKID, Dns.GetHostName());
        }

        /// <summary>
        /// Removes the stale device codes.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task RemoveDeviceCodesAsync(CancellationToken cancellationToken = default)
        {
            var found = int.MaxValue;

            if (!await IsJobReservedAsync(REMOVE_DEVICECODES_TASKID, Dns.GetHostName(), cancellationToken))
                return;

            while (found >= _options.TokenCleanupBatchSize)
            {
                var expiredCodes = await _context.DeviceFlowCodes.AsQueryable().Where(x => x.Expiration < DateTime.UtcNow)
                                                                                .OrderBy(x => x.DeviceCode)
                                                                                .Select(x => x.DeviceCode)
                                                                                .Take(_options.TokenCleanupBatchSize)
                                                                                .ToListAsync(cancellationToken);

                found = expiredCodes.Count;

                _logger.LogInformation("Removing {DeviceCodeCount} device flow codes", found);

                if (found > 0)
                {
                    try
                    {
                        var requests = new List<WriteModel<DeviceFlowCode>>();
                        var filter = Builders<DeviceFlowCode>.Filter.In(x => x.DeviceCode, expiredCodes);
                        requests.Add(new DeleteManyModel<DeviceFlowCode>(filter));
                        await _context.DeviceFlowCodes.BulkWriteAsync(requests, new BulkWriteOptions { BypassDocumentValidation = true, IsOrdered = true }, cancellationToken);

                        if (_operationalStoreNotification != null)
                            await _operationalStoreNotification.PersistedGrantsRemovedAsync(expiredCodes, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogDebug("Concurrency exception removing expired device codes: {exception}", ex.Message);
                    }
                }
            }

            await ClearJobReservationAsync(REMOVE_DEVICECODES_TASKID, Dns.GetHostName());
        }

        private async Task RemovePersistedGrantsAsync(IList<string> grants, CancellationToken cancellationToken = default)
        {
            var requests = new List<WriteModel<PersistedGrant>>();
            var filter = Builders<PersistedGrant>.Filter.In(x => x.Key, grants);
            requests.Add(new DeleteManyModel<PersistedGrant>(filter));
            await _context.PersistedGrants.BulkWriteAsync(requests, new BulkWriteOptions { BypassDocumentValidation = true, IsOrdered = true }, cancellationToken);
        }

        private async Task<bool> IsJobReservedAsync(string taskId, string workerId, CancellationToken cancellationToken = default)
        {
            var filter = Builders<JobSchedule>.Filter.Eq(x => x.TaskId, taskId)
                & (Builders<JobSchedule>.Filter.Eq(x => x.WorkerId, null) & Builders<JobSchedule>.Filter.Eq(x => x.WorkerExpiration, null))
                | (Builders<JobSchedule>.Filter.Lt(x => x.WorkerExpiration, DateTime.UtcNow));
            var update = Builders<JobSchedule>.Update.Set(x => x.WorkerId, workerId).Set(x => x.WorkerExpiration, DateTime.UtcNow.AddSeconds(_options.WorkerExpiration));
            var result = await _scheduler.Jobs.FindOneAndUpdateAsync(filter, update, new FindOneAndUpdateOptions<JobSchedule> { IsUpsert = false, ReturnDocument = ReturnDocument.After }, cancellationToken);
            var reserved = result?.WorkerId == workerId;

            _logger.LogDebug("Reserving Job {TaskId} to {WorkerId}: {Reserved}", taskId, workerId, reserved);

            return reserved;
        }

        private async Task ClearJobReservationAsync(string taskId, string workerId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Cleaning Job Reservation for {TaskId}", taskId);

            var filter = Builders<JobSchedule>.Filter.Eq(x => x.TaskId, taskId) & Builders<JobSchedule>.Filter.Eq(x => x.WorkerId, workerId);
            var update = Builders<JobSchedule>.Update.Set(x => x.WorkerId, null).Set(x => x.WorkerExpiration, null);
            await _scheduler.Jobs.UpdateOneAsync(filter, update, cancellationToken: cancellationToken);
        }
    }
}
