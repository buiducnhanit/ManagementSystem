using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.ConfigureMiddlewaresAsync();

app.Run();
