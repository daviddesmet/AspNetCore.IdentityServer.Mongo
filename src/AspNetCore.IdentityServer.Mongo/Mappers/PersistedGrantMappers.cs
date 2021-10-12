namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for persisted grants.
    /// </summary>
    public static class PersistedGrantMappers
    {
        static PersistedGrantMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<PersistedGrantMapperProfile>()).CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.PersistedGrant? ToModel(this Entities.PersistedGrant entity)
            => entity == null ? null : Mapper.Map<Duende.IdentityServer.Models.PersistedGrant>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.PersistedGrant? ToEntity(this Duende.IdentityServer.Models.PersistedGrant model)
            => model == null ? null : Mapper.Map<Entities.PersistedGrant>(model);

        /// <summary>
        /// Updates an entity from a model.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="entity">The entity.</param>
        public static void UpdateEntity(this Duende.IdentityServer.Models.PersistedGrant model, Entities.PersistedGrant entity)
            => Mapper.Map(model, entity);
    }
}
