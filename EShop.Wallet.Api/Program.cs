using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Wallet.Api.Handlers;
using EShop.Wallet.DataProvider.Repositories;
using EShop.Wallet.DataProvider.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();

var rabbitmqConfig = new RabbitMqOption();
builder.Configuration.GetSection("rabbitmq").Bind(rabbitmqConfig);
// Connect to rabbitmq using masstransit
builder.Services.AddMassTransit(x =>
{
    // Adds all consumers from the same namespace
    x.AddConsumersFromNamespaceContaining<AddFundsConsumer>();

    x.AddBus(provider=> Bus.Factory.CreateUsingRabbitMq(cfg =>
    {
        cfg.Host(new Uri(rabbitmqConfig.ConnectionString), hostConfig =>
        {
            hostConfig.Username(rabbitmqConfig.Username);
            hostConfig.Password(rabbitmqConfig.Password);
        });
        
        cfg.ReceiveEndpoint("add_funds", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<AddFundsConsumer>(provider);
        });
        
        cfg.ReceiveEndpoint("deduct_funds", ep =>
        {
            ep.PrefetchCount = 16;
            ep.UseMessageRetry(retryConfig => { retryConfig.Interval(2, 100); });
            ep.ConfigureConsumer<DeductFundsConsumer>(provider);
        });
    }));
});

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

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

var dbInitializer = app.Services.GetService<IDatabaseInitializer>();
dbInitializer.InitializeAsync();

app.Run();
