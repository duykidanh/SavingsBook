using System.Text;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SavingsBook.Application.AutoMapperProfile;
using SavingsBook.Application.Contracts.FileUploadClient;
using SavingsBook.Application.FileUploadClient;
using SavingsBook.Application.Redis;
using SavingsBook.Infrastructure.Authentication;
using SavingsBook.Infrastructure.MongoDbConfig;
using SavingsBook.Infrastructure.RolesConfig;

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

#region Register custom services

// builder.Services.AddScoped<IStoreService, StoreService>();
// builder.Services.AddScoped<IStoreCategoryService, StoreCategoryService>();


builder.Services.AddScoped<RedisCacheService>();
builder.Services.AddScoped<IFileUploadClient, FileUploadClient>();

#endregion



builder.Services.ConfigureMongoDbIdentity<ApplicationUser, ApplicationRole, Guid>(mongoDbConfig)
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
        ClockSkew = TimeSpan.Zero
    };
}).AddIdentityCookies();

#endregion



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleSeeder = services.GetRequiredService<RoleSeeder>();
    await roleSeeder.SeedRoles();
}*/

app.Run();