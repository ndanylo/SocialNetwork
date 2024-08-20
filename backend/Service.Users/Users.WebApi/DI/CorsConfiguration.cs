namespace Users.WebApi.DI
{
    public static class CorsConfiguration
    {
        public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsPolicyName = configuration["Cors:PolicyName"] ?? "DefaultPolicy";
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new string[] { };
    
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });
        }

        public static void UseCorsMiddleware(this IApplicationBuilder app, IConfiguration configuration)
        {
            var corsPolicyName = configuration["Cors:PolicyName"] ?? "DefaultPolicy";
            app.UseCors(corsPolicyName);
        }
    }
}
