using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.Models;
using Microsoft.OpenApi.Models;
using PmsApi.Utilities;
using PmsApi.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("PmsContext");
Console.WriteLine($"Connection string: {connectionString}");
var serverVersion = ServerVersion.AutoDetect(connectionString);
var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:5073/api/users",
                                              "http://localhost:4200");
                          policy.AllowAnyMethod();
                          policy.AllowAnyHeader();
                      });
});

// Add services to the container.
builder.Services
.AddIdentityApiEndpoints<User>()
.AddRoles<Role>()
//.AddIdentity<User, Role>()
.AddEntityFrameworkStores<PmsContext>()
.AddApiEndpoints().AddDefaultTokenProviders();
builder.Services.AddAuthorization( opt => 
{
    opt.AddPolicy("IsAdmin", p => p.RequireRole(["Admin"]));
    opt.AddPolicy("IsSuperAdmin", p => p.RequireClaim("SuperAdmin"));
});
builder.Services.AddDbContext<PmsContext>( opt => 
    opt.UseMySql(connectionString, serverVersion));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;  //per ignorare la ricorsività del json
});
builder.Services.AddScoped<IUserContextHelper, UserContextHelper>();
builder.Services.AddScoped<IProjectService, ProjectService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( opt =>
{
    opt.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme{ 
        In = ParameterLocation.Header, 
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            []
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapIdentityApi<User>();

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
