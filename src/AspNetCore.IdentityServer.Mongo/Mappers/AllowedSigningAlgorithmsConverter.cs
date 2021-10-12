namespace AspNetCore.IdentityServer.Mongo.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;

    internal class AllowedSigningAlgorithmsConverter : IValueConverter<ICollection<string>, string?>, IValueConverter<string?, ICollection<string>>
    {
        public static readonly AllowedSigningAlgorithmsConverter Converter = new();

        public string? Convert(ICollection<string> sourceMember, ResolutionContext context)
        {
            if (sourceMember is null || !sourceMember.Any())
                return null;

            return sourceMember.Aggregate((x, y) => $"{x},{y}");
        }

        public ICollection<string> Convert(string? sourceMember, ResolutionContext context)
        {
            var list = new HashSet<string>();
            if (!string.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                    list.Add(item);
            }
            return list;
        }
    }
}
