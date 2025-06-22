using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseUrls("http://*:80");

// Add services to the container.
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Configuration.AddEnvironmentVariables();

// Add Ocelot services
builder.Services.AddOcelot(builder.Configuration);
//builder.Services.AddSwaggerForOcelot(builder.Configuration);

// Configure JWT Authentication for Ocelot
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
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
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    return context.Response.WriteAsync("{\"error\":\"TokenExpired\"}");
                }

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\":\"InvalidToken\"}");
            }
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

//app.UseSwaggerForOcelotUI(options =>
//{
//    options.PathToSwaggerGenerator = "/swagger/docs";
//});
await app.UseOcelot();

app.Run();
    