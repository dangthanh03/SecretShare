using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SecretShare.Models.Domains;
using SecretShare.Models.Service.Abstract;
using SecretShare.Models.Service.Implementation;
using SecretShare.Service.Abstract;
using SecretShare.Service.Implementation;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
var cloudinaryConfig = builder.Configuration.GetSection("CloudinarySettings");
var cloudinaryAccount = new Account(
    cloudinaryConfig["CloudName"],
    cloudinaryConfig["ApiKey"],
    cloudinaryConfig["ApiSecret"]
);

var cloudinary = new Cloudinary(cloudinaryAccount);

builder.Services.AddSingleton(cloudinary);
builder.Services.AddTransient<ICloudinaryService, CloudinaryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFileService,FileService>();
builder.Services.AddSingleton<ICacheService, CacheService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
