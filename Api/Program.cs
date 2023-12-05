using Api;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
builder.Services.AddSingleton<IProducer<Null, string>>(x => new ProducerBuilder<Null, string>(config).Build());
builder.Services.AddDbContext<DataDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("conn")));

builder.Services.AddSingleton<IMessagePublisher, MessagePublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SensorController");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
