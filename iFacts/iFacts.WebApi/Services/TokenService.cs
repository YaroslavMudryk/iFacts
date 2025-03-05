using iFacts.Data;
using iFacts.Data.Entities;
using iFacts.Shared.Auth;
using iFacts.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace iFacts.WebApi.Services;

public interface ITokenService
{
    Task<JwtToken> GetUserTokenAsync(UserTokenModel userToken);
}

public class TokenService(FactsDbContext db) : ITokenService
{
    public async Task<JwtToken> GetUserTokenAsync(UserTokenModel userToken)
    {
        userToken.Lang = userToken.Lang.ToLower();

        var user = userToken.User ?? await db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == userToken.UserId);

        if (user == null)
            return null;

        var currentUserRoles = await db.UserRoles
            .Where(x => x.UserId == user.Id)
            .Include(x => x.Role)
            .Select(x => x.Role)
            .ToListAsync();

        var claims = new List<System.Security.Claims.Claim>
        {
            new(AppClaims.Types.SessionId, userToken.SessionId.ToString()),
            new(AppClaims.Types.Login, user.Login),
            new(AppClaims.Types.UserId, user.Id.ToString()),
        };

        foreach (var role in currentUserRoles)
        {
            claims.Add(new System.Security.Claims.Claim(AppClaims.Types.Role, role.Name));
        }

        var roleIds = currentUserRoles.Select(x => x.Id);

        var roleClaims = await db.RoleClaims.Where(s => roleIds.Contains(s.RoleId)).Include(s => s.Claim).ToListAsync();

        var userRoleClaims = GetUniqClaims(roleClaims);

        if (userRoleClaims != null && userRoleClaims.Count > 0)
        {
            foreach (var roleClaim in userRoleClaims)
            {
                claims.Add(new System.Security.Claims.Claim(roleClaim.Type, roleClaim.Value));
            }
        }

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
        var now = DateTime.Now;
        var expiredAt = now.Add(TimeSpan.FromMinutes(TokenOptions.LifeTimeInMinutes));
        var jwt = new JwtSecurityToken(
                issuer: TokenOptions.Issuer,
                audience: TokenOptions.Audience,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: expiredAt,
                signingCredentials: new SigningCredentials(TokenOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return new JwtToken
        {
            AccessToken = encodedJwt,
            ExpiredAt = expiredAt,
            SessionId = userToken.SessionId.ToString()
        };
    }

    private static List<ClaimPermissionDto> GetUniqClaims(List<RoleClaim> roleClaims)
    {
        return [.. roleClaims.Select(s => new ClaimPermissionDto(s.Claim.Type, s.Claim.Value)).Distinct()];
    }
}