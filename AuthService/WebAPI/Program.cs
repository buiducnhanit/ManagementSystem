using Infrastructure.DependencyInjection;
using ManagementSystem.Shared.Common.DependencyInjection;

var builder = WebApplication.CreateBuilder(args); 

builder.Host.ConfigureSerilog();
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInfrastructureService(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseInfrastructurePolicy();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
