using EShop.Infrastructure.Activities.RoutingActivities;
using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Order.Api.Handlers;
using EShop.Order.DataProvider.Repositories;
using EShop.Order.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

var rabbitMq = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitMq);
// establish connection with RabbitMQ...
builder.Services.AddMassTransit(x => {
    // Point to consumer to handles commands
    x.AddConsumersFromNamespaceContaining<CreateOrderHandler>();
    // Add activities in order to execute slip
    x.AddActivitiesFromNamespaceContaining<RoutingActivities>();
    x.SetKebabCaseEndpointNameFormatter();

    // Returns Bus Control obj using RabbitMQ
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        // Connection details of rabbitMQ
        cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg => {
            hostcfg.Username(rabbitMq.Username);
            hostcfg.Password(rabbitMq.Password);
        });

        //Map endpoints and set configuration
        cfg.ConfigureEndpoints(provider);
    }));
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Version = "v1",
        Title = "EShop Order API Endpoints",
        Description = "These API Endpoints are available to CRUD Order related data"
    });
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API");
    options.RoutePrefix = string.Empty;
});

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

var dbInitializer = app.Services.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
