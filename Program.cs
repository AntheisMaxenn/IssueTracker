using IssueTracker.Authorization;
using IssueTracker.Cache;
using IssueTracker.Contracts.HealthChecks;
using IssueTracker.Data;
using IssueTracker.Filters;
using IssueTracker.HealthChecks;
using IssueTracker.Mapping;
using IssueTracker.Options;
using IssueTracker.Services;
using IssueTracker.Services.Implementations;
using IssueTracker.Services.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

// Serilog bootstrap logger.
Log.Logger = new LoggerConfiguration()
             .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
             .Enrich.FromLogContext()
             .WriteTo.Console()
             .CreateBootstrapLogger();

try{
    #region Logging Startup
    Log.Information("Server starting now");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        );
    #endregion


    // Add services to the container.

    builder.Services.AddControllers(options =>
    {
        // This is a global filter, Applied to all Endpoints under this app.
        // options.Filters.Add<GlobalActionFilter>();
        //options.Filters.AddService<ActionFilterAttribute>();
    });


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

    #region Swagger + jwt support

    builder.Services.AddSwaggerGen(x => {

        var security = new Dictionary<string, IEnumerable<string>>
        {
            {"Bearer", new string[0] }
        };

        x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "standard"
        });
        x.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                new string[0] {}
                }
            });
        
        //x.AddSecurityRequirement();
    });
    #endregion

    #region Jwt

    var jwtSettings = new JwtSettings();
    builder.Configuration.Bind(nameof(jwtSettings), jwtSettings);
    builder.Services.AddSingleton(jwtSettings);

    //var connectionString = new ConnectionStrings();
    //builder.Configuration.Bind(nameof(connectionString), connectionString);
    //builder.Services.AddSingleton(connectionString);

    // Makes the ConnectionStrings Available for DI.
    builder.Services.Configure<ConnectionStrings>(builder.Configuration.GetSection("ConnectionStrings"));
    builder.Services.Configure<SuperAdminStrings>(builder.Configuration.GetSection("SuperAdminAccout"));

    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,

    };

    builder.Services.AddSingleton(tokenValidationParameters);

    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        x.SaveToken = true;
        x.TokenValidationParameters = tokenValidationParameters;
    });

    #endregion

    #region DbContext
    builder.Services.AddDbContext<IssueTrackerDbContext>(options => options.UseSqlServer(
            builder.Configuration["ConnectionStrings:DefaultConnection"]));

    builder.Services.AddDefaultIdentity<IdentityUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<IssueTrackerDbContext>();
    #endregion


    #region Services + DI
    builder.Services.AddSingleton<ActionFilterAttribute>();
    builder.Services.AddScoped<IMachineService, MachineService>();
    builder.Services.AddScoped<IComponentService, ComponentService>();
    builder.Services.AddScoped<ILocationService, LocationService>();
    builder.Services.AddScoped<IIssueService, IssueService>();
    builder.Services.AddScoped<IActionService, ActionService>();

    builder.Services.AddScoped<IIdentityService, IdentityService>();
    builder.Services.AddScoped<IssueTracker.Services.IAuthorizationService, AuthorizationService>();

    //builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IHttpContextAccessor, HttpContextAccessor>();
    #endregion

    #region Authorization and Policys
    builder.Services.AddAuthorization(optionss =>
    {
        optionss.AddPolicy("IsDotComEmail", policy =>
        {
            policy.AddRequirements(new IsDotComEmailRequirement(".com"));
        });
    });
    builder.Services.AddSingleton<IAuthorizationHandler, IsDotComEmailHandler>();
    #endregion

    #region Mapping
    builder.Services.AddAutoMapper(typeof(RequestToDomainProfile));
    #endregion

    #region Cache
    var redisCacheSettings = new RedisCacheSettings();
    builder.Configuration.GetSection(nameof(RedisCacheSettings)).Bind(redisCacheSettings);
    builder.Services.AddSingleton(redisCacheSettings);

    if (redisCacheSettings.isEnabled)
    {
        builder.Services.AddStackExchangeRedisCache(options => options.Configuration = redisCacheSettings.ConnectionString);
        builder.Services.AddSingleton<IResponseCacheService, ResponseCacheService>();

    }
    #endregion

    #region HealthChecks
    builder.Services.AddHealthChecks()
        .AddDbContextCheck<IssueTrackerDbContext>()
        .AddCheck<RedisHealthCheck>("Redis");
    builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisCacheSettings.ConnectionString));
    #endregion

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
    });


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        // Development
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        // Production
        app.UseSwagger();
        app.UseSwaggerUI();

        var log = new LoggerConfiguration()
        .WriteTo
        .ApplicationInsights(TelemetryConfiguration.Active, TelemetryConverter.Traces)
        .CreateLogger();


    }

    #region Middleware

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new HealthCheckResponse
            {
                Status = report.Status.ToString(),
                Checks = report.Entries.Select(x => new HealthCheck
                {
                    Component = x.Key,
                    Status = x.Value.Status.ToString(),
                    Description = x.Value.Description
                }),
                Duration = report.TotalDuration
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            
        }
    });
    app.UseAuthorization();
    app.MapControllers();

    app.UseCors();
    #endregion

    #region Seeding SuperAdmin

    using (var scope = app.Services.CreateScope())
    {
        // Seedings roles and SuperAdmin.

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var useranager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await roleManager.RoleExistsAsync("SuperAdmin"))
        {
            var adminRole = new IdentityRole("SuperAdmin");
            await roleManager.CreateAsync(adminRole);
        }

        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            var adminRole = new IdentityRole("Admin");
            await roleManager.CreateAsync(adminRole);
        }

        if (!await roleManager.RoleExistsAsync("User"))
        {
            var adminRole = new IdentityRole("User");
            await roleManager.CreateAsync(adminRole);
        }

        var superAdminExists = await useranager.FindByEmailAsync(builder.Configuration["SuperAdminAccout:Email"]);

        var superAdmin = new IdentityUser
        {
            Email = builder.Configuration["SuperAdminAccout:Email"],
            UserName = builder.Configuration["SuperAdminAccout:Email"]
        };

        if (superAdminExists == null)
        {
            await useranager.CreateAsync(superAdmin, builder.Configuration["SuperAdminAccout:Password"]);
            await useranager.AddToRoleAsync(superAdmin, "SuperAdmin");

        }

    }

    #endregion

    app.Run();

}catch (Exception e)
{

    Log.Fatal(e + " There was a fatal error.");
    return 1;

}
finally
{

    Log.CloseAndFlush();

}

return 0;