using iFacts.Data.Entities;
using iFacts.WebApi.Dtos;
using Riok.Mapperly.Abstractions;

namespace iFacts.WebApi;

[Mapper]
public static partial class AppMapper
{
    [MapProperty(nameof(Session.Client), nameof(SessionDto.Device))]
    public static partial SessionDto MapToDto(this Session session);

    public static partial FactDto MapToDto(this Fact fact);
}
