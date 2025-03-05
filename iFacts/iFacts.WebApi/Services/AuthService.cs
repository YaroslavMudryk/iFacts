using Extensions.DeviceDetector;
using Extensions.Password;
using iFacts.Data;
using iFacts.Data.Entities;
using iFacts.Shared.Auth;
using iFacts.Shared.Exceptions;
using iFacts.WebApi.Dtos;
using Microsoft.EntityFrameworkCore;

namespace iFacts.WebApi.Services;

public class AuthService(
    IUserContext userContext,
    FactsDbContext factsDbContext,
    TimeProvider timeProvider,
    IDetector detector,
    ILocationService locationService,
    ITokenService tokenService)
{
    public async Task RegisterAsync(RegisterDto register)
    {
        if (await factsDbContext.Users.AnyAsync(s => s.Login == register.Login))
            throw new BadRequestException("User with same login already registerd");

        var newUser = new User(register.FirstName, register.MiddleName, lastName: register.LastName, register.Login)
        {
            PasswordHash = register.Password.GeneratePasswordHash()
        };
        await factsDbContext.Users.AddAsync(newUser);
        await factsDbContext.SaveChangesAsync();

        var role = await factsDbContext.Roles.AsNoTracking().FirstOrDefaultAsync(s => s.Name == AppConstants.Roles.User);

        var userRole = new UserRole
        {
            UserId = newUser.Id,
            RoleId = role.Id
        };

        await factsDbContext.UserRoles.AddAsync(userRole);
        await factsDbContext.SaveChangesAsync();
    }

    public async Task<JwtToken> LoginAsync(LoginDto loginDto)
    {
        var app = await factsDbContext.Apps
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ClientId == loginDto.App.Id && x.ClientSecret == loginDto.App.Secret);

        if (app == null)
            throw new NotFoundException("App not found");

        if (app.IsActive)
            throw new BadRequestException("App isn`t configured");

        var userToLogin = await factsDbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Login == loginDto.Login);
        if (userToLogin == null)
            throw new NotFoundException("User not found");

        if (!loginDto.Password.VerifyPasswordHash(userToLogin.PasswordHash))
        {
            throw new BadRequestException("Password is incorrect");
        }

        if (loginDto.Client == null)
            loginDto.Client = detector.GetClientInfo();

        var location = await locationService.GetIpInfoAsync(userContext.IpV4);
        
        var appDb = new AppModel
        {
            Id = app.Id,
            Name = app.Name,
            ShortName = app.ShortName,
            Image = app.Image,
            Description = app.Description,
            Version = loginDto.App.Version
        };

        var sessionId = Guid.NewGuid();

        var session = new Session
        {
            Id = sessionId,
            IsActive = true,
            App = appDb,
            Client = loginDto.Client,
            Location = location,
            UserId = userToLogin.Id,
            PushFcmToken = loginDto.PushFcmToken,
            ExpiredAt = timeProvider.GetUtcNow().UtcDateTime.AddDays(60)
        };

        var accessTokenModel = await tokenService.GetUserTokenAsync(new UserTokenModel
        {
            AuthType = "pwd",
            UserId = userToLogin.Id,
            Lang = loginDto.Lang,
            SessionId = sessionId
        });

        var refreshToken = Guid.NewGuid().ToString("N");

        session.RefreshToken = refreshToken;
        session.Type = AppScheme.Password;
        session.ViaMFA = false;

        await factsDbContext.Sessions.AddAsync(session);

        await factsDbContext.SaveChangesAsync();

        //_sessionManager.AddSession(new TokenModel(accessTokenModel.AccessToken, accessTokenModel.ExpiredAt));

        return new JwtToken
        {
            AccessToken = accessTokenModel.AccessToken,
            ExpiredAt = accessTokenModel.ExpiredAt,
            RefreshToken = refreshToken,
            SessionId = session.Id.ToString()
        };
    }

    public async Task LogoutAsync()
    {
        var currentUser = userContext.AssumeAuthenticated<BasicAuthenticatedUser>();
        await RevokeSessionsAsync([currentUser.SessionId]);
    }

    public async Task RevokeSessionsAsync(IReadOnlyList<string> sessionIds)
    {
        var currentUser = userContext.AssumeAuthenticated<BasicAuthenticatedUser>();

        var sessionsToRevoke = await factsDbContext
            .Sessions
            .Where(s => sessionIds
                .Select(s => Guid.Parse(s))
                    .Contains(s.Id))
            .ToListAsync();

        CheckIfAvalbelUserSessions(sessionsToRevoke, currentUser);

        foreach (var session in sessionsToRevoke)
        {
            session.IsActive = false;
            session.DeactivatedBySessionId = Guid.Parse(currentUser.SessionId);
            session.DeactivatedAt = timeProvider.GetUtcNow().UtcDateTime;
        }
        factsDbContext.Sessions.UpdateRange(sessionsToRevoke);
        await factsDbContext.SaveChangesAsync();
    }

    private static void CheckIfAvalbelUserSessions(List<Session> sessions, BasicAuthenticatedUser currentUser)
    {
        var isUserSessions = sessions.All(s => s.UserId == currentUser.UserId);
        if (!isUserSessions)
            throw new ForbiddenException("User is trying to revoke someone else's session");
    }

    public async Task<SessionsResponse> GetUserSessionsAsync()
    {
        var currentUser = userContext.AssumeAuthenticated<BasicAuthenticatedUser>();

        var userSessions = await factsDbContext.Sessions.AsNoTracking().Where(s => s.UserId == currentUser.UserId && s.IsActive).ToListAsync();

        return new SessionsResponse
        {
            Sessions = GetSortedSessions(userSessions, currentUser.SessionId)
        };
    }

    private static IReadOnlyList<SessionDto> GetSortedSessions(List<Session> sessions, string currentSessionId)
    {
        var sortedSessions = new List<SessionDto>
        {
            sessions.FirstOrDefault(s => s.Id == Guid.Parse(currentSessionId)).MapToDto()
        };

        sortedSessions.AddRange(
            sessions
            .OrderByDescending(s => s.CreatedAt)
            .Where(s => s.Id != Guid.Parse(currentSessionId))
            .Select(s => s.MapToDto()));

        return sortedSessions;
    }
}
