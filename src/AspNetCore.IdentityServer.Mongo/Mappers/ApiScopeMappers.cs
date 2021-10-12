namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for api scopes.
    /// </summary>
    public static class ScopeMappers
    {
        static ScopeMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiScopeMapperProfile>()).CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.ApiScope? ToModel(this Entities.ApiScope entity)
            => entity == null ? null : Mapper.Map<Duende.IdentityServer.Models.ApiScope>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ApiScope? ToEntity(this Duende.IdentityServer.Models.ApiScope model)
            => model == null ? null : Mapper.Map<Entities.ApiScope>(model);
    }
}
