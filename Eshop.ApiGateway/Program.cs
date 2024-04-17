using Eshop.ApiGateway.DTO;
using Eshop.ApiGateway.Middleware;
using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.EventBus;
using MassTransit;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

public class Program
{
    public static void Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false)
            .AddJsonFile("Ocelot.config.json", false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();


        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddRabbitMq(configuration);
        builder.Services.AddJwt(configuration);
        builder.Services.Configure<AsyncRoutesOption>(configuration.GetSection("AsyncRoutes"));
        // Ocelot
        builder.Services.AddOcelot(configuration).AddCacheManager(settings=>settings.WithDictionaryHandle());

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
        }
        app.UseStaticFiles();

        var busControl = app.Services.GetService<IBusControl>();
        busControl.Start();

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseOcelotQueueMiddleware();

        app.UseOcelot().Wait();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}