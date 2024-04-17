using EShop.Cart.Api.Handlers;
using EShop.Cart.DataProvider.Repository;
using EShop.Cart.DataProvider.Services;
using EShop.Infrastructure.EventBus;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache((cfg) =>
{
    cfg.Configuration = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
});
// Specify singleton cart repository
builder.Services.AddSingleton<ICartRepository, CartRepository>();
builder.Services.AddSingleton<ICartService, CartService>();

var options = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(options);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AddCartItemConsumer>();
    x.AddConsumer<RemoveCartItemConsumer>();

    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(options.ConnectionString), hostConfig =>
        {
            hostConfig.Username(options.Username);
            hostConfig.Password(options.Password);
        });
        //Creates endpoint "add_cart"
        cfg.ReceiveEndpoint("add_cart", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<AddCartItemConsumer>(provider);
        });
        // Creates endpoint "remove_cart"
        cfg.ReceiveEndpoint("remove_cart", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<RemoveCartItemConsumer>(provider);
        });
    }));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

// To access the consumers you need to init the bus
var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

app.Run();
