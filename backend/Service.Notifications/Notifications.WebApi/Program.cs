using Notifications.Application.Hubs;
using Notifications.WebApi.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplicationServices();
builder.Services.AddAutoMapperServices();
builder.Services.AddDbContextServices();

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddMediatRServices();

builder.Services.AddMassTransitConfig(builder.Configuration);
builder.Services.ConfigureCors(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCorsMiddleware(builder.Configuration);
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapHub<ChatHub>("/chat");

app.Run();
