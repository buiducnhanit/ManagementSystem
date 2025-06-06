using WebAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:80");
// Add services to the container.
builder.ConfigureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
await app.ConfigureMiddlewaresAsync();

app.Run();
    