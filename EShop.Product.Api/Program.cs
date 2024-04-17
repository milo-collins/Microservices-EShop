using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Product.Api.Handlers;
using EShop.Product.DataProvider.Repositories;
using EShop.Product.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
//    options => 
//        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);
builder.Services.AddMongoDb(builder.Configuration);

// Add service / repo dependency 
builder.Services.AddScoped<IProductRepository,ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<CreateProductHandler>();

var rabbitMq = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitMq);
// establish connection with RabbitMQ...
builder.Services.AddMassTransit(x => {
    // Point to consumer to handles commands
    x.AddConsumer<CreateProductHandler>();

    // Returns Bus Control obj using RabbitMQ
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        // Connection details of rabbitMQ
        cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg => {
            hostcfg.Username(rabbitMq.Username);
            hostcfg.Password(rabbitMq.Password);
        });

        //Map endpoints and set configuration
        cfg.ReceiveEndpoint("create_product", ep =>
        {
            // 16 Msgs can be processed at a time.
            ep.PrefetchCount = 16;
            // If messages fail to process, retry twice with 100ms interval
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            // This endpoint will be handled by the specified handler
            ep.ConfigureConsumer<CreateProductHandler>(provider);
        });
    }));
});

var app = builder.Build();

//app.UseHttpsRedirection();

//app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(ep =>
{
    ep.MapControllers();
});

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

var dbInitializer = app.Services.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

app.Run();
