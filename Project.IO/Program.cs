using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Business.Interfaces;
using Business.Services;
using DataAccess;
using IO;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Business.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        options.SerializerSettings.Culture = System.Globalization.CultureInfo.InvariantCulture;
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    })
    .AddJsonOptions(option => {
        option.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Add configurations DbContext
builder.Services.AddDbContext<Context>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnections"));
});

// Add configurations Identity
builder.Services.AddIdentity<User, Role>(option => {
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireUppercase = false;
    option.Password.RequiredLength = 2;
}).AddEntityFrameworkStores<Context>().AddDefaultTokenProviders();

// Add Hosted Services
builder.Services.AddHostedService<LifeTime>();

// Add Services
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddScoped<ITest, TestService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// Add configurations authentication and authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            RequireExpirationTime = true,
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(builder.Configuration["JWT_Secret1"]!)),
            SaveSigninToken = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

// Add Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder => {
        builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .SetIsOriginAllowed((Host) => true)
        .AllowCredentials()
        .WithExposedHeaders(new string[] { "content-disposition" });
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo {
        Title = "Nombre de App",
        Version = "v1",
        Description = "Descripción de la App",
        Contact = new OpenApiContact
        {
            Email = "pqrsf@corrosioncic.com",
            Name = "CIC - Corporación para la Investigacion de la Corrosión",
            Url = new Uri("https://corrosion.uis.edu.co/webcic/index.php/es/sobre-la-cic/quienes-somos")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
        }
    });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
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
            new string[]{}
        }
    });
    var fileXML = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var routXML = Path.Combine(AppContext.BaseDirectory, fileXML);
    options.IncludeXmlComments(routXML);
});

// Build the App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("all");

app.UseRouting();

app.UseAuthorization();

app.UseAuthentication();

app.MapControllers();

app.Run();