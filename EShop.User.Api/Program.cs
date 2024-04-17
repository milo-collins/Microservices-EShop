using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Infrastructure.Security;
using EShop.User.Api.Handlers;
using EShop.User.DataProvider;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Add Repository and Service
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddSingleton<IEncrypter, Encrypter>();

var rabbitMq = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitMq);
// establish connection with RabbitMQ...
builder.Services.AddMassTransit(x => {
    x.AddConsumer<CreateUserHandler>();
    // Returns Bus Control obj using RabbitMQ
    x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        // Connection details of rabbitMQ
        cfg.Host(new Uri(rabbitMq.ConnectionString), hostcfg => {
            hostcfg.Username(rabbitMq.Username);
            hostcfg.Password(rabbitMq.Password);
        });

        //Map endpoints and set configuration
        cfg.ReceiveEndpoint("add_user", ep =>
        {
            // 16 Msgs can be processed at a time.
            ep.PrefetchCount = 16;
            // If messages fail to process, retry twice with 100ms interval
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            // Configure Handler as the consumer for the Add user queue
            ep.ConfigureConsumer<CreateUserHandler>(provider);
        });
    }));
});

// Add mongo db connectivity
builder.Services.AddMongoDb(builder.Configuration);

var app = builder.Build();

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

var dbInitializer = app.Services.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

app.Run();
