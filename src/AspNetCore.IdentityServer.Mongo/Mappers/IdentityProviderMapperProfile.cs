namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using System.Collections.Generic;
    using System.Text.Json;
    using AutoMapper;
    using Duende.IdentityServer.Models;

    /// <summary>
    /// Defines entity/model mapping for identity provider.
    /// </summary>
    /// <seealso cref="AutoMapper.Profile" />
    public class IdentityProviderMapperProfile : Profile
    {
        /// <summary>
        /// <see cref="IdentityProviderMapperProfile"/>
        /// </summary>
        public IdentityProviderMapperProfile() => CreateMap<Entities.IdentityProvider, IdentityProvider>(MemberList.Destination)
                .ForMember(x => x.Properties, opts => opts.ConvertUsing(PropertiesConverter.Converter, x => x.Properties))
                .ReverseMap()
                .ForMember(x => x.Properties, opts => opts.ConvertUsing(PropertiesConverter.Converter, x => x.Properties));

        private class PropertiesConverter : IValueConverter<Dictionary<string, string>, string?>, IValueConverter<string?, Dictionary<string, string>>
        {
            public static readonly PropertiesConverter Converter = new();

            public string Convert(Dictionary<string, string> sourceMember, ResolutionContext context) => JsonSerializer.Serialize(sourceMember);

            public Dictionary<string, string> Convert(string? sourceMember, ResolutionContext context)
            {
                if (string.IsNullOrWhiteSpace(sourceMember))
                    return new Dictionary<string, string>();

                return JsonSerializer.Deserialize<Dictionary<string, string>>(sourceMember)!;
            }
        }
    }
}
