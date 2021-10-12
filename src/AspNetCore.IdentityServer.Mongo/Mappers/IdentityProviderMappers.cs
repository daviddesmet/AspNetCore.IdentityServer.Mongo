namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for identity providers.
    /// </summary>
    public static class IdentityProviderMappers
    {
        static IdentityProviderMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityProviderMapperProfile>())
                .CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.IdentityProvider? ToModel(this Entities.IdentityProvider entity)
            => entity == null ? null : Mapper.Map<Duende.IdentityServer.Models.IdentityProvider>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.IdentityProvider? ToEntity(this Duende.IdentityServer.Models.IdentityProvider model)
            => model == null ? null : Mapper.Map<Entities.IdentityProvider>(model);
    }
}
