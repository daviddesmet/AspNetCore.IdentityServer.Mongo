namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Extension methods to map to/from entity/model for clients.
    /// </summary>
    public static class ClientMappers
    {
        static ClientMappers() => Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>()).CreateMapper();

        internal static IMapper Mapper { get; }

        /// <summary>
        /// Maps an entity to a model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public static Duende.IdentityServer.Models.Client ToModel(this Entities.Client entity)
            => Mapper.Map<Duende.IdentityServer.Models.Client>(entity);

        /// <summary>
        /// Maps a model to an entity.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static Entities.Client ToEntity(this Duende.IdentityServer.Models.Client model)
            => Mapper.Map<Entities.Client>(model);
    }
}
