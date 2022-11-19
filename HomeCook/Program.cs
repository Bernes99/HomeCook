using FluentMigrator.Runner;
using HomeCook;
using HomeCook.Data;
using HomeCook.Data.Extensions;
using HomeCook.Data.Models;
using HomeCook.Data.Seeders;
using HomeCook.Services;
using HomeCook.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("secret.json", optional: false, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultDatabase");

LogManager.LoadConfiguration(string.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// Add services to the container.

//Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.SignIn.RequireConfirmedEmail = false;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
}).AddEntityFrameworkStores<DefaultDbContext>().AddDefaultTokenProviders();

//JWT
var authSettings = new AuthenticationSettings();
builder.Configuration.GetSection("Authentication").Bind(authSettings);

var authBuilder = builder.Services.AddAuthentication(authentication =>
{
    authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    //authentication.DefaultAuthenticateScheme = "Bearer";
    //authentication.DefaultChallengeScheme = "Bearer";
    //authentication.DefaultScheme = "Bearer";
})
.AddCookie()
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateActor = false,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidIssuer = authSettings.JwtIssuer,
        ValidAudience = authSettings.JwtIssuer,
        ClockSkew = TimeSpan.Zero, // czas po wygaœnieciu tokena przez który nadal bêdzie autoryzowany
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authSettings.JwtKey))
    };
    options.Events = new JwtBearerEvents();
    options.Events.OnMessageReceived = context =>
    {

        if (context.Request.Cookies.ContainsKey("X-Access-Token"))
        {
            context.Token = context.Request.Cookies["X-Access-Token"];
        }

        return Task.CompletedTask;
    };
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = ".AspNetCore.Identity.Application";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;

    options.Cookie.Name = "X-Refresh-Token";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;

    options.Cookie.Name = "X-Access-Token";
    options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    options.SlidingExpiration = true;
});

builder.Services.AddSingleton(authSettings);
//loger manager
builder.Services.ConfigureLoggerService();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeCook API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
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
        new String []{ }
        }
    });
});

//Servisy
builder.Services.AddDbContext<DefaultDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(connectionString)); //connectionString
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // to correctly save dateTime to postgres db
//builder.Services.AddDbContext<AppIdentityDbContext>(optionsBuilder => optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDatabase")));
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWeatherForecastService, WeatherForecastService>();
builder.Services.AddScoped<RoleSeeder>();

//to google auth
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    // This lambda determines whether user consent for non-essential cookies is needed for a given request.
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
});

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddCors(p => p.AddPolicy("CORSPolicy", buid => {
    buid.WithOrigins("http://localhost:5173", "https://www.youtube.com/").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

//dodawanie servisu fluentMigratora
var serviceProvider = ServicesRegistration.CreateFluentService(connectionString);
using (var scope = serviceProvider.CreateScope())
{
    // Instantiate the runner
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();

    // Execute the migrations
    runner.MigrateUp();
    //runner.MigrateDown(0);

}

var app = builder.Build();

// Tutaj dodawaj middleware do pipelina
var scopeServices = app.Services.CreateScope();
var seeder = scopeServices.ServiceProvider.GetRequiredService<RoleSeeder>();
//seeder.Seed();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CORSPolicy");
app.UseAuthentication();

//app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.ConfigureCustomExceptionMiddleware();

app.Run();
