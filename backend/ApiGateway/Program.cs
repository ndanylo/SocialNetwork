using ApiGateway.DI;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Configuration.AddJsonFile("Properties/ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.ConfigureCors(builder.Configuration);

var app = builder.Build();

app.UseCorsMiddleware(builder.Configuration);
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
await app.UseOcelot();

app.Run();
