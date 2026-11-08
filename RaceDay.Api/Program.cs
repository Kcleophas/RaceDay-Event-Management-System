
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using RaceDay.Api.Data;
using RaceDay.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Register application services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<BlobStorageService>();

// Configure SQL Server database
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("RaceDayConnection")));

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// JWT configuration
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is missing.");

var signingKey = new SymmetricSecurityKey(
    Convert.FromBase64String(jwtKey));

// Configure JWT authentication
builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // JWT diagnostic checks
        options.Events = new JwtBearerEvents
        {
            // Check whether the API receives the token
            OnMessageReceived = context =>
            {
                bool tokenReceived =
                    !string.IsNullOrWhiteSpace(
                        context.Request.Headers.Authorization);

                Console.WriteLine(
                    "JWT TOKEN RECEIVED: " +
                    (tokenReceived ? "YES" : "NO"));

                return Task.CompletedTask;
            },

            // Display authentication validation errors
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine(
                    "JWT AUTHENTICATION FAILED: " +
                    context.Exception.Message);

                return Task.CompletedTask;
            },

            // Display authorization challenge details
            OnChallenge = context =>
            {
                Console.WriteLine(
                    "JWT CHALLENGE: " +
                    context.Error + " - " +
                    context.ErrorDescription);

                return Task.CompletedTask;
            }
        };

        // Token validation settings
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateLifetime = true,

                ClockSkew = TimeSpan.FromMinutes(1)
            };
    });

// Configure authorization
builder.Services.AddAuthorization();

// Build application
var app = builder.Build();

// Configure HTTP request pipeline
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseSession();

// Authentication must run before authorization
app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Start application
app.Run();
