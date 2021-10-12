namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using System.Collections.Generic;
    using AutoMapper;

    /// <summary>
    /// Defines entity/model mapping for identity resources.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class IdentityResourceMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceMapperProfile"/> class.
        /// </summary>
        public IdentityResourceMapperProfile()
        {
            CreateMap<Entities.IdentityResourceProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Entities.IdentityResource, Duende.IdentityServer.Models.IdentityResource>(MemberList.Destination)
                .ConstructUsing(src => new Duende.IdentityServer.Models.IdentityResource())
                .ReverseMap();

            CreateMap<Entities.IdentityResourceClaim, string>()
               .ConstructUsing(x => x.Type)
               .ReverseMap()
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));
        }
    }
}
