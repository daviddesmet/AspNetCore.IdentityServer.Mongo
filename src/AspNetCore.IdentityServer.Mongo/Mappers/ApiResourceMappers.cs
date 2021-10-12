namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for API resources.
    /// </summary>
    public static class ApiResourceMappers
    {
        static ApiResourceMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>()).CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.ApiResource? ToModel(this Entities.ApiResource entity)
            => entity == null ? null : Mapper.Map<Duende.IdentityServer.Models.ApiResource>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.ApiResource? ToEntity(this Duende.IdentityServer.Models.ApiResource model)
            => model == null ? null : Mapper.Map<Entities.ApiResource>(model);
    }
}
