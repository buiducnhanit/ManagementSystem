using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Infrastructure.DependencyInjection;
using ManagementSystem.Shared.Common.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebAPI.Configure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.ConfigureSerilog();
builder.Services.AddInfrastructureService(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        await Infrastructure.Data.SeedData.InitializeAsync(services);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ICustomLogger<Program>>();
//        logger.Error("An error occurred while initializing the database.", ex);
//        throw;
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in descriptions.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseInfrastructurePolicy();

app.Run();
