using Backend.DAL;
using Backend.Models.Felhasznalo;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

namespace Backend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
            builder.Services.AddScoped<AppDbContext>();
            builder.Services.AddCors(static (CorsOptions options) => {
                options.AddDefaultPolicy(static (CorsPolicyBuilder builder) => {
                    builder
                        .WithOrigins(
                            "http://localhost:5500",
                            "http://127.0.0.1:5500"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                    ;
                });
            });
            builder.Services
                .AddAuthentication(static (AuthenticationOptions options) => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer((JwtBearerOptions options) => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"]!,
                        ValidAudience = builder.Configuration["Jwt:Audience"]!,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                    };
                })
            ;
            builder.Services.AddAuthorization(static (AuthorizationOptions options) => {
                string[] engedelyNevek = Enum.GetNames<Felhasznalo.Engedely>();
                for (int i = 0; i < engedelyNevek.Length; i++)
                {
                    string engedelyNev = engedelyNevek[i];
                    options.AddPolicy(engedelyNev, (AuthorizationPolicyBuilder builder) => {
                        builder.RequireClaim(engedelyNev, "true");
                    });
                }
            });
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            WebApplication app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            public void Configure(SwaggerGenOptions options)
            {
                const string Bearer = nameof(Bearer);
                options.AddSecurityDefinition(Bearer, new OpenApiSecurityScheme {
                    In = ParameterLocation.Header,
                    Description = "Provide a valid token, or I'll hire a pair of scissors to cut of your balls",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = Bearer
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = Bearer
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
        }
    }
}
