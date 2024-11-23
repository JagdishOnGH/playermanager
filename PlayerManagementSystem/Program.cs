using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PlayerManagementSystem.EfContext;
using PlayerManagementSystem.Helper;
using PlayerManagementSystem.Helpers;
using PlayerManagementSystem.Models;
using PlayerManagementSystem.Models.AuthModel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                // Suppress the default behavior
                context.HandleResponse();

                // Customize your response
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var response = SharedHelper.CreateErrorResponse("You are not authorized to access this resource");
                return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
        
    });


// Configure DbContext with PostgreSQL
builder.Services.AddDbContext<EfDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database")));
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

// Configure Identity with AppUser and IdentityRole
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 10;
}).AddEntityFrameworkStores<EfDbContext>();

// Register JwtHelper as a Scoped service
builder.Services.AddScoped<JwtHelper>();

// Configure controllers with custom behavior for model validation
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

// Build the app
var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Enable Authentication and Authorization
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// Map controllers
app.MapControllers();

// Seed roles if necessary
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedRoles(services);
}

// Run the app
app.Run();

// SeedRoles function to ensure roles are created and removed as necessary
async Task SeedRoles(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Define the roles
    var roles = new List<string>
    {
        TerritoryType.Municipality.ToString(),
        TerritoryType.District.ToString(),
        TerritoryType.Province.ToString(),
        TerritoryType.Ward.ToString(),
        "SuperAdmin"
    };

    // Fetch existing roles from the database
    var existingRoles = roleManager.Roles.Select(r => r.Name).ToList();

    // Add missing roles
    foreach (var role in roles)
    {
        if (!existingRoles.Contains(role))
        {
            var identityRole = new IdentityRole(role)
            {
                NormalizedName = role.ToUpperInvariant()
            };
            await roleManager.CreateAsync(identityRole);
        }
    }

    // Remove roles no longer in use
    foreach (var role in existingRoles)
    {
        if (!roles.Contains(role))
        {
            var roleToDelete = await roleManager.FindByNameAsync(role);
            if (roleToDelete != null)
            {
                await roleManager.DeleteAsync(roleToDelete);
            }
        }
    }
}
