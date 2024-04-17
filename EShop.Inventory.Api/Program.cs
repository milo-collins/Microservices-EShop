using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Inventory.Api.Handlers;
using EShop.Inventory.DataProvider.Repositories;
using EShop.Inventory.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();

var rabbitmqConfig = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqConfig);
// Connect to rabbitmq using masstransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AllocateProductConsumer>();
    x.AddConsumer<ReleaseProductConsumer>();

    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqConfig.ConnectionString), hostConfig =>
        {
            hostConfig.Username(rabbitmqConfig.Username);
            hostConfig.Password(rabbitmqConfig.Password);
        });

        cfg.ReceiveEndpoint("allocate_product", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<AllocateProductConsumer>(provider);
        });
        
        cfg.ReceiveEndpoint("release_product", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<ReleaseProductConsumer>(provider);
        });
    }));
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "EShop Inventory API Endpoints",
        Description = "These API Endpoints are available to CRUD Stock related data"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory API");
    options.RoutePrefix = string.Empty;
});
app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();

app.Run();
