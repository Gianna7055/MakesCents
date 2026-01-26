using AutoMapper;
using MakesCentsBackend.Services.BusinessLogicLayer;
using MakesCentsBackend.Services.DataAccessLayer;
using MakesCentsBackend.Services.Mappers;
using MakesCentsBackend.Services.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;
using System.Data;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add a scoped DI for the connection string
builder.Services.AddScoped<IDbConnection>(sp =>
    new MySqlConnection(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper with DI
builder.Services.AddAutoMapper(config =>
{
    config.AddProfile<UserMappingProfile>();
}, AppDomain.CurrentDomain.GetAssemblies());

// Add a scoped logic classes that will persist for each request
builder.Services.AddScoped<UserLogic>();
builder.Services.AddScoped<BudgetLogic>();

// Add a scoped DAO classes that will persist for each request
builder.Services.AddScoped<UserDAO>();
builder.Services.AddScoped<BudgetDAO>();

// Get the JWT key and issuer
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

// Register auth services and set JWT Bearer fo the auth scheme
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    // Add JWT configuration
    .AddJwtBearer(options =>
    {
        // Define security rules for incoming JWTs
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            // Make sure Makes Cents created the token
            ValidateIssuer = true,
            // Set the allowed issuer (Makes Cents)
            ValidIssuer = jwtIssuer,
            // Skip the audience claim
            ValidateAudience = false,
            // Validates the tokens expiration
            ValidateLifetime = true,
            // Make sure the token was signed by my secret key
            ValidateIssuerSigningKey = true,
            // Convert the secret string to a cryptographic key
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
            // Prevent clock skew confusion
            ClockSkew = TimeSpan.Zero
        };
    });
// Enable Authorize and AllowAnonymous
builder.Services.AddAuthorization();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add a scoped JWT service class that will persist for each request
builder.Services.AddScoped<JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Read the Auth header and validates JWT
app.UseAuthentication();

// Enforces Authorize rules
app.UseAuthorization();

app.MapControllers();

app.Run();
