using EShop.Infrastructure.Authentication;
using EShop.Infrastructure.EventBus;
using EShop.Infrastructure.Mongo;
using EShop.Infrastructure.Security;
using EShop.User.DataProvider;
using EShop.User.Query.Api.Handlers;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IEncrypter, Encrypter>();
// Mongo Conectivity
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddJwt(builder.Configuration);
// Add Services, Repos, Handlers
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<LoginUserHandler>();
builder.Services.AddScoped<IAuthenticationHandler, AuthenticationHandler>();

// establish connection with RabbitMQ...
builder.Services.AddMassTransit(x => {
    // Returns Bus Control obj using RabbitMQ
    x.AddConsumer<LoginUserHandler>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

var dbInitializer = app.Services.GetRequiredService<IDatabaseInitializer>();
await dbInitializer.InitializeAsync();

var busControl = app.Services.GetService<IBusControl>();
busControl.Start();

app.UseEndpoints(ep =>
{
    ep.MapControllers();
});

app.Run();
