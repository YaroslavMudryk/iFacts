using Extensions.DeviceDetector;
using iFacts.Data;
using iFacts.Data.Audit;
using iFacts.Shared.Auth;
using iFacts.WebApi.Infrastructure.Middlewares;
using iFacts.WebApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace iFacts.WebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<IUserContext, UserContext>();
        builder.Services.AddDbContext<FactsDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
        });
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<AuditRepo>();
        builder.Services.AddScoped<FactsService>();
        builder.Services.AddDeviceDetector();
        builder.Services.AddScoped<ILocationService, LocationService>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddScoped<ITokenResolverService, MockTokenResolverService>();
        builder.Services.AddControllers();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = false;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = TokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = TokenOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = TokenOptions.GetSymmetricSecurityKey(),
                    ValidateIssuerSigningKey = true,
                };
            });

        builder.Services.AddTransient<CorrelationContextMiddleware>();
        builder.Services.AddTransient<AuthenticationMiddleware>();
        builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddResponseCompression(options => options.EnableForHttps = true);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "(Ukrainian Remote Learning System) API",
            });


            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "JWT Authentication",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Description = "Put your JWT Bearer token on textbox below!",

                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };

            s.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            s.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                { jwtSecurityScheme, Array.Empty<string>() }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<CorrelationContextMiddleware>();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<AuthenticationMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
