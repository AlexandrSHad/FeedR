using FeedR.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHttpContextAccessor();

var app = builder.Build();

app.UseCorrelationId();
app.MapGet("/", () => "FeedR News feed");

app.Run();