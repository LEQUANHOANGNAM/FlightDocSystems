using FlightDocSystem.Data;
using FlightDocSystem.Service;
using FlightDocSystem.Services.Implementations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===================== DB =====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// ===================== SERVICES =====================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserSVC, UserSVC>();
builder.Services.AddScoped<IRoleSVC, RoleSVC>();
builder.Services.AddScoped<IPermissionSVC, PermissionSVC>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IFlightSvc, FlightSvc>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<IFlightDocumentService, FlightDocumentService>();
builder.Services.AddScoped<IDocumentCategoryService, DocumentCategoryService>();    
builder.Services.AddScoped<IFlightAssignmentService,FlightAssignmentService>();
builder.Services.AddMemoryCache();
builder.Services.AddAuthorization();

// ===================== SWAGGER =====================
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FlightDocSystem API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// ===================== AUTH =====================
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
            ),

            ClockSkew = TimeSpan.Zero
        };

        // 🔥 CHECK TOKEN REVOKED
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var cache = context.HttpContext.RequestServices
                    .GetRequiredService<IMemoryCache>();

                var db = context.HttpContext.RequestServices
                    .GetRequiredService<AppDbContext>();

                var token = context.Request.Headers["Authorization"]
                    .ToString()
                    .Replace("Bearer ", "");

                // memory
                if (cache.TryGetValue(token, out _))
                {
                    context.Fail("Token revoked");
                    return;
                }

                // database
                var revoked = await db.RevokedTokens
                    .AnyAsync(x => x.Token == token);

                if (revoked)
                {
                    context.Fail("Token revoked");
                }
            },

            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("JWT FAILED:");
                Console.WriteLine(context.Exception);
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
