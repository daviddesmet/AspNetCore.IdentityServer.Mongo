namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for identity resources.
    /// </summary>
    public static class IdentityResourceMappers
    {
        static IdentityResourceMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>()).CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.IdentityResource? ToModel(this Entities.IdentityResource entity)
            => entity == null ? null : Mapper.Map<Duende.IdentityServer.Models.IdentityResource>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.IdentityResource? ToEntity(this Duende.IdentityServer.Models.IdentityResource model)
            => model == null ? null : Mapper.Map<Entities.IdentityResource>(model);
    }
}
