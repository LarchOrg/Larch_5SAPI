using Microsoft.EntityFrameworkCore;
using _5sAudit.Data;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer; 
using Microsoft.IdentityModel.Tokens;               
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Removes the default 5-minute grace period for expiry
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Using new() here is safe for simple metadata
    c.SwaggerDoc("v1", new() { Title = "5S Audit API", Version = "v1" });
});

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://5saudit.larcherp.com", "http://5sAuditapi.larcherp.com") // React dev URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); ;
        });
});
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "5S Audit API v1");
        c.RoutePrefix = "swagger";
    });

    app.MapOpenApi();
    app.MapScalarApiReference();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseCors("AllowReactDev");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Handle preflight
//app.MapMethods("{*path}", new[] { "OPTIONS" }, () => Results.Ok());

app.Run();
