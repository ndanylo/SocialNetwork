using Users.WebApi.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMediatRServices();
builder.Services.AddApplicationServices();
builder.Services.AddDbContextServices();
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddMassTransitConfig(builder.Configuration);
builder.Services.AddSwaggerGen();
builder.Services.ConfigureCors(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwaggerUI();
    app.UseSwagger();
}

app.UseStaticFiles();
app.UseCorsMiddleware(builder.Configuration);
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();
