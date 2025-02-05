using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using SavingsBook.Application.AutoMapperProfile;
using SavingsBook.Application.Contracts.Authentication;
using SavingsBook.Application.Contracts.FileUploadClient;
using SavingsBook.Application.Contracts.SavingBook;
using SavingsBook.Application.FileUploadClient;
using SavingsBook.Application.Redis;
using SavingsBook.HostApi.Middleware;
using SavingsBook.Infrastructure.MongoDbConfig;
using SavingsBook.Application.RolesConfig;
using Microsoft.OpenApi.Models;
using SavingsBook.HostApi.Utility;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configurator = builder.Configuration;


#region config

builder.Services.AddAutoMapper(typeof(MapperProfile));
builder.Services.AddStackExchangeRedisCache(opts =>
{
    opts.Configuration = configurator["Redis:Configuration"];
});

var mongoDbConfig = new MongoDbIdentityConfiguration()
{
    MongoDbSettings = new MongoDbSettings
    {
        ConnectionString = configurator["MongoDbSettings:ConnectionStrings"],
        DatabaseName = configurator["MongoDbSettings:DatabaseName"]
    },
    IdentityOptionsAction = options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;

        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        options.Lockout.MaxFailedAccessAttempts = 5;

        options.User.RequireUniqueEmail = true;
    }
};




builder.Services.InitMongoCollections();

#region Register application services

builder.Services.AddHostedService<BalanceUpdateService>();

builder.Services.AddScoped<ISavingBookService, SavingBookService>();
builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddScoped<IFileUploadClient, FileUploadClient>();
/*builder.Services.AddScoped<ISavingRegulationService, SavingRegulationService>();*/
builder.Services.AddScoped<ISavingBookService, SavingBookService>();


#endregion



builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, ObjectId>(mongoDbConfig)
    .AddUserManager<UserManager<ApplicationUser>>()
    .AddSignInManager<SignInManager<ApplicationUser>>()
    .AddRoleManager<RoleManager<ApplicationRole>>()
    .AddDefaultTokenProviders();
builder.Services.AddTransient<RoleSeeder>();

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = true;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidAudience = configurator["JWT:ValidAudience"],
        ValidIssuer = configurator["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurator["JWT:Secret"])),
        /*ClockSkew = TimeSpan.Zero*/
    };
}).AddIdentityCookies();

#endregion

builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Thêm Bearer token vào header
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
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
            new string[] { }
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger().RequireAuthorization();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opts =>
{
    opts.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.UseMiddleware<GlobalMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleSeeder = services.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRoles();
}

app.Run();