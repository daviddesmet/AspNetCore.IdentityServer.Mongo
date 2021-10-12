namespace AspNetCore.IdentityServer.Mongo.Stores
{
    using System;
    using System.Threading.Tasks;

    using Duende.IdentityServer.Models;
    using Duende.IdentityServer.Stores;
    using Duende.IdentityServer.Stores.Serialization;
    using IdentityModel;
    using Microsoft.Extensions.Logging;
    using MongoDB.Driver.Linq;
    using MongoDB.Driver;

    using AspNetCore.IdentityServer.Mongo.Entities;
    using AspNetCore.IdentityServer.Mongo.Interfaces;

    /// <summary>
    /// Implementation of IDeviceFlowStore thats uses MongoDB.
    /// </summary>
    /// <seealso cref="IDeviceFlowStore" />
    public class DeviceFlowStore : IDeviceFlowStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceFlowStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="serializer">The serializer</param>
        /// <param name="logger">The logger.</param>
        public DeviceFlowStore(IPersistedGrantDbContext context, IPersistentGrantSerializer serializer, ILogger<DeviceFlowStore> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The DbContext.
        /// </summary>
        protected IPersistedGrantDbContext Context { get; private set; }

        /// <summary>
        ///  The serializer.
        /// </summary>
        protected IPersistentGrantSerializer Serializer { get; private set; }

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger<DeviceFlowStore> Logger { get; private set; }

        /// <summary>
        /// Stores the device authorization request.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public virtual async Task StoreDeviceAuthorizationAsync(string deviceCode, string userCode, DeviceCode data)
            => await Context.DeviceFlowCodes.InsertOneAsync(ToEntity(data, deviceCode, userCode));

        /// <summary>
        /// Finds device authorization by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <returns></returns>
        public virtual async Task<DeviceCode?> FindByUserCodeAsync(string userCode)
        {
            var deviceFlowCode = await Context.DeviceFlowCodes.AsQueryable().Where(x => x.UserCode == userCode).SingleOrDefaultAsync();
            var model = ToModel(deviceFlowCode?.Data);

            Logger.LogDebug("{UserCode} found in database: {UserCodeFound}", userCode, model != null);

            return model;
        }

        /// <summary>
        /// Finds device authorization by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public virtual async Task<DeviceCode?> FindByDeviceCodeAsync(string deviceCode)
        {
            var deviceFlowCode = await Context.DeviceFlowCodes.AsQueryable().Where(x => x.DeviceCode == deviceCode).SingleOrDefaultAsync();
            var model = ToModel(deviceFlowCode?.Data);

            Logger.LogDebug("{DeviceCode} found in database: {DeviceCodeFound}", deviceCode, model != null);

            return model;
        }

        /// <summary>
        /// Updates device authorization, searching by user code.
        /// </summary>
        /// <param name="userCode">The user code.</param>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public virtual async Task UpdateByUserCodeAsync(string userCode, DeviceCode data)
        {
            var existing = await Context.DeviceFlowCodes.AsQueryable().Where(x => x.UserCode == userCode).SingleOrDefaultAsync();
            if (existing is null)
            {
                Logger.LogError("{UserCode} device code not found in database", userCode);
                throw new InvalidOperationException("Could not update device code");
            }

            var entity = ToEntity(data, existing.DeviceCode, userCode);
            Logger.LogDebug("{UserCode} device code found in database", userCode);

            var update = Builders<DeviceFlowCode>.Update
                                                 .Set(x => x.SubjectId, data.Subject.FindFirst(JwtClaimTypes.Subject)!.Value)
                                                 .Set(x => x.Data, entity.Data);

            var result = await Context.DeviceFlowCodes.UpdateOneAsync(x => x.UserCode == userCode, update);
            if (result.ModifiedCount == 0)
            {
                Logger.LogDebug("No {UserCode} device code found in database", userCode);
                return;
            }

            Logger.LogDebug("Updated {UserCode} device code from database", userCode);
        }

        /// <summary>
        /// Removes the device authorization, searching by device code.
        /// </summary>
        /// <param name="deviceCode">The device code.</param>
        /// <returns></returns>
        public virtual async Task RemoveByDeviceCodeAsync(string deviceCode)
        {
            Logger.LogDebug("Removing {DeviceCode} device code from database", deviceCode);

            var result = await Context.DeviceFlowCodes.DeleteOneAsync(x => x.DeviceCode == deviceCode);
            if (result.DeletedCount == 0)
            {
                Logger.LogDebug("No {DeviceCode} device code found in database", deviceCode);
                return;
            }

            Logger.LogDebug("Removed {DeviceCode} device code from database", deviceCode);
        }

        /// <summary>
        /// Converts a model to an entity.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="deviceCode"></param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        protected DeviceFlowCode ToEntity(DeviceCode model, string deviceCode, string userCode) => new()
        {
            DeviceCode = deviceCode,
            UserCode = userCode,
            ClientId = model.ClientId,
            SubjectId = model.Subject?.FindFirst(JwtClaimTypes.Subject)!.Value,
            CreationTime = model.CreationTime,
            Expiration = model.CreationTime.AddSeconds(model.Lifetime),
            Data = Serializer.Serialize(model)
        };

        /// <summary>
        /// Converts a serialized DeviceCode to a model.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected DeviceCode? ToModel(string? entity)
        {
            if (entity is null)
                return null;

            return Serializer.Deserialize<DeviceCode>(entity);
        }
    }
}
