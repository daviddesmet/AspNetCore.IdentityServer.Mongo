namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using AutoMapper;

    /// <summary>
    /// Defines entity/model mapping for persisted grants.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class PersistedGrantMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="PersistedGrantMapperProfile">
        /// </see>
        /// </summary>
        public PersistedGrantMapperProfile()
            => CreateMap<Entities.PersistedGrant, Duende.IdentityServer.Models.PersistedGrant>(MemberList.Destination).ReverseMap();
    }
}
