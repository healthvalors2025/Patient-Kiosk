using DoctorMobileApp.CommonClass;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PatientKiosk.Middlewares;
using PatientKiosk.Validators;
using PatientKiosk.WebServices;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IDbConnectionFactory, SqlHelper>();
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,   
        Scheme = "bearer",                
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token. Example: 12345abcdef"
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
            new string[] {}
        }
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var expClaim = context.Principal?.FindFirst("exp")?.Value;

            if (!string.IsNullOrEmpty(expClaim))
            {
                var expUtc = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim)).UtcDateTime;
                TimeZoneInfo indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime expIndia = TimeZoneInfo.ConvertTimeFromUtc(expUtc, indiaZone);
                DateTime nowIndia = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaZone);
                var remainingMinutes = (expIndia - nowIndia).TotalMinutes;
                int warningMinutes = Convert.ToInt32(builder.Configuration["Jwt:WarningMinutes"]);
                if (remainingMinutes <= warningMinutes && remainingMinutes > 0)
                {
                    context.Response.Headers.Append("X-Token-Expiring","true");
                    context.Response.Headers.Append("X-Token-Remaining-Minutes",Math.Ceiling(remainingMinutes).ToString());
                    context.Response.Headers.Append("X-Token-Expiry-IST",expIndia.ToString("dd-MM-yyyy hh:mm:ss tt"));
                }
                if (remainingMinutes <= 0)
                {
                    context.Fail("Token expired");
                }
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException ex)
            {
                TimeZoneInfo indiaZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                DateTime expIndia = TimeZoneInfo.ConvertTimeFromUtc(ex.Expires.ToUniversalTime(),indiaZone);
                context.Response.Headers.Append("Token-Expired-Time-IST",expIndia.ToString("dd-MM-yyyy hh:mm:ss tt"));
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddAuthorization();
var assembly = Assembly.GetExecutingAssembly();

var serviceTypes = assembly.GetTypes()
    .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"));
foreach (var serviceType in serviceTypes)
{
    builder.Services.AddScoped(serviceType);
}
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();