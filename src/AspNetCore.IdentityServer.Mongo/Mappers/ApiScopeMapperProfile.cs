namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using System.Collections.Generic;
    using AutoMapper;

    /// <summary>
    /// Defines entity/model mapping for api scopes.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class ApiScopeMapperProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiScopeMapperProfile"/> class.
        /// </summary>
        public ApiScopeMapperProfile()
        {
            CreateMap<Entities.ApiScopeProperty, KeyValuePair<string, string>>()
                .ReverseMap();

            CreateMap<Entities.ApiScopeClaim, string>()
               .ConstructUsing(x => x.Type)
               .ReverseMap()
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src));

            CreateMap<Entities.ApiScope, Duende.IdentityServer.Models.ApiScope>(MemberList.Destination)
                .ConstructUsing(src => new Duende.IdentityServer.Models.ApiScope())
                .ForMember(x => x.Properties, opts => opts.MapFrom(x => x.Properties))
                .ForMember(x => x.UserClaims, opts => opts.MapFrom(x => x.UserClaims))
                .ReverseMap();
        }
    }
}
