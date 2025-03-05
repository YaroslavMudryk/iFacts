using iFacts.Data;
using iFacts.Shared.Auth;
using iFacts.Shared.Exceptions;
using iFacts.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace iFacts.WebApi.Services;

public class FactsService(
    IUserContext userContext,
    FactsDbContext factsDbContext)
{
    public async Task<FactDto> GetRandomFactAsync()
    {
        var randomId = new Random().Next(1, 100000);
        return await GetFactByIdAsync(randomId);
    }

    public async Task<FactDto> GetFactByIdAsync(int factId)
    {
        userContext.AssumeAuthenticated<BasicAuthenticatedUser>()
            .EnsureUserHasPermissions(AppClaims.Types.Facts, AppClaims.Values.View);

        var fact = await factsDbContext.Facts.AsNoTracking().SingleOrDefaultAsync(s => s.Id == factId);
        if (fact is null)
            throw new NotFoundException("Fact not found");

        return fact.MapToDto();
    }
}
