using TimeTaskTracking.Core.Mapper;
using TimeTaskTracking.Core.Extensions;
using TimeTaskTracking.Extensions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using TimeTaskTracking.Shared.ViewModels.Utility;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.StaticFiles;
using TimeTaskTracking.Validators;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure logging
builder.Logging.ClearProviders();   
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Services.AddControllers();
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//     {
//         options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
//     });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SchemaFilter<SwaggerIgnoreFilter>();
    c.OperationFilter<IgnorePropertyFilter>();
    //c.UseOneOfForPolymorphism();
    c.UseAllOfToExtendReferenceSchemas();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TimeTaskTracking", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.ApiKey,
        Name = "X-API-KEY",
        In = ParameterLocation.Header,
        Description = "API key needed to access the endpoints."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                },
                new string[] {}
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddCoreDependencies();
builder.Services.AddApiDependencies();

builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});
//JWT Authentication
var appSettingSection = builder.Configuration.GetSection("JWTSettings");
builder.Services.Configure<JWTSettings>(appSettingSection);
var appSetting = appSettingSection.Get<JWTSettings>();
var key = Encoding.UTF8.GetBytes(appSetting.Secret);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(x =>
{
    //x.RequireHttpsMetadata = false;
    //x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidIssuer = appSetting.Issuer,
        ValidAudience = appSetting.Audience,
        //ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOrAdminOrHOD", policy => policy.RequireRole("Project Manager", "Admin", "HOD"));
    options.AddPolicy("TeamLeadOrAdmin", policy => policy.RequireRole("Team Lead", "Admin"));
    options.AddPolicy("EmployeeOrTeamLeadOrBDMOrAdmin", policy => policy.RequireRole("Employee", "Team Lead", "BDM", "Admin"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrBDMOrAdminOrHOD", policy => policy.RequireRole("Project Manager", "BDM", "Admin", "HOD"));
    options.AddPolicy("ManagerOrHROrAdminOrHOD", policy => policy.RequireRole("Project Manager", "HR", "Admin", "HOD"));
    options.AddPolicy("ManagerOrTeamLeadOrBDMOrAdminOrHOD", policy => policy.RequireRole("Project Manager", "Team Lead", "BDM", "Admin", "HOD"));
    options.AddPolicy("EmployeeOrTeamLeadOrManager", policy => policy.RequireRole("Project Manager", "Team Lead", "Employee"));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TeamLeadOrManagerOrAdmin", policy => policy.RequireRole("Team Lead", "Admin", "Project Manager"));
    options.AddPolicy("EmployeeOrTeamLeadOrBDM", policy => policy.RequireRole("Employee", "Team Lead", "BDM"));
    options.AddPolicy("HROnly", policy => policy.RequireRole("HR"));
    options.AddPolicy("ManagerOrEmployee", policy => policy.RequireRole("Project Manager", "Employee"));
    options.AddPolicy("TeamLeadOrManagerOrAdminOrHROrHOD", policy => policy.RequireRole("Team Lead", "Admin", "Project Manager", "HR","HOD"));
    options.AddPolicy("EmployeeOrTeamLeadOrManagerOrBDM", policy => policy.RequireRole("Project Manager", "Team Lead", "Employee", "BDM"));
    options.AddPolicy("TeamLeadOrManagerOrAdminOrHOD", policy => policy.RequireRole("Team Lead", "Admin", "Project Manager", "HOD"));
    options.AddPolicy("TeamLeadOrManagerOrAdminOrHODOrHR", policy => policy.RequireRole("Team Lead", "Admin", "Project Manager", "HOD", "HR"));
    options.AddPolicy("EmployeeOrBDM", policy => policy.RequireRole("Employee", "BDM"));
    options.AddPolicy("ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHOD", policy => policy.RequireRole("Project Manager", "Team Lead", "BDM", "Admin", "Employee", "HOD"));
    options.AddPolicy("ManagerOrTeamLeadOrBDMOrAdminOrEmployeeOrHODOrHR", policy => policy.RequireRole("Project Manager", "Team Lead", "BDM", "Admin", "Employee", "HOD", "HR"));
    options.AddPolicy("AdminOrHOD", policy => policy.RequireRole("Admin", "HOD"));
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));

});

var app = builder.Build();

app.UseCors("AllowAll");
// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        
        c.DocExpansion(DocExpansion.List);
    });

    app.UseStaticFiles();

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "images")
    ),
        RequestPath = "/images"
    });

    app.UseDirectoryBrowser(new DirectoryBrowserOptions
    {
        FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "images")
    ),
        RequestPath = "/images"
    });
}
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(builder.Environment.WebRootPath, "images")
//    ),
//    RequestPath = "/images"
//});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//        Path.Combine(Directory.GetCurrentDirectory(), "ProfileImages")
//    ),
//    RequestPath = "/ProfileImages"
//});
app.UseMiddleware<ApiKeyMiddleware>();
app.MapControllers();

app.Run();


