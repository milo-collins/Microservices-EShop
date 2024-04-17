using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Product.DataProvider.Repositories;
using EShop.Product.DataProvider.Services;
using EShop.Product.Query.Api.Handlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Mongo Conectivity
builder.Services.AddMongoDb(builder.Configuration);
// Add Services, Repos, Handlers
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<GetProductByIdHandler>();

// establish connection with RabbitMQ...
builder.Services.AddMassTransit(x => {
    // Returns Bus Control obj using RabbitMQ
    x.AddConsumer<GetProductByIdHandler>();
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        var rabbitMq = new RabbitMqOption();
        // Binds to rabbit mq option class object
        builder.Configuration.GetSection("rabbitmq").Bind(rabbitMq);
        // Connection details of rabbitMQ
        cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg => {
            hostcfg.Username(rabbitMq.Username);
            hostcfg.Password(rabbitMq.Password);
        });
        cfg.ConfigureEndpoints(provider);
    }));
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

var bus = app.Services.GetService<IBusControl>();
bus.Start();

var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer?.InitializeAsync();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
