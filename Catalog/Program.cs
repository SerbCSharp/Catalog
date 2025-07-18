using Catalog.Application.Interfaces;
using Catalog.Infrastructure.EventBus.RabbitMQ;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RabbitMq
builder.Services.Configure<RabbitMqConfiguration>(builder.Configuration.GetSection(RabbitMqConfiguration.Section));
builder.Services.AddSingleton<IAsyncConnectionFactory>(provider =>
{
    var factory = new ConnectionFactory
    {
        HostName = builder.Configuration["RabbitMq:Host"],
        DispatchConsumersAsync = true,
        AutomaticRecoveryEnabled = true,
        ConsumerDispatchConcurrency = 1 // Configure the amount of concurrent consumers within one host
    };
    return factory;
});
builder.Services.AddSingleton<IConnectionProvider, ConnectionProvider>();
builder.Services.AddSingleton<IChannelProvider, ChannelProvider>();
builder.Services.AddSingleton<IEventSend, Send>();
builder.Services.AddHostedService<Receive>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
