using ApiVending.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ApiVending.Models;
using Microsoft.OpenApi.Models;
using static ApiVending.Controllers.AuthController;
using ApiVending.Services.Companies;
using ApiVending.Services.Users;
using ApiVending.Services.ServerTask;
using ApiVending.Services.Sales;
using ApiVending.Services.Product;
using ApiVending.Services.TA;
using Microsoft.AspNetCore.Diagnostics;

namespace ApiVending
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
     
            builder.Services.AddDbContext<VendingSystemDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();
            
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidAudience = jwtOptions.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.Key))
                    };
                });

            builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
            builder.Services.AddScoped<IJwtService, JwtService>();

            builder.Services.AddAuthorization();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IUsersServices, UserService>();
            builder.Services.AddScoped<IServerTask, ServiceTaskService>();
            builder.Services.AddScoped<ISales, SalesService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IMachineService, TAs>();
            var app = builder.Build();  

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler(appError => appError.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;

                var (status, message) = exception switch
                {
                    KeyNotFoundException e => (404, e.Message),
                    InvalidOperationException e => (409, e.Message),
                    ArgumentException e => (400, e.Message),
                    _ => (500, "Внутренняя ошибка сервера")
                };

                context.Response.StatusCode = status;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new { message });
            }));
            
            app.UseHttpsRedirection();
            app.Use(async (context, next) =>
        {
            var token = context.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
    
            if (!string.IsNullOrEmpty(token) && TokenBlacklist.IsBlacklisted(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Токен инвалидирован");
                return;
            }
            await next();
        });
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}