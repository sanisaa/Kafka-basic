using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Add this namespace for IConfiguration
using System;
using System.Threading.Tasks;
using Api.Model;

var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
using var producer = new ProducerBuilder<Null, string>(config).Build();

// Assuming you have IConfiguration for configuration management
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
    .AddJsonFile("appsettings.json")
    .Build();

var optionsBuilder = new DbContextOptionsBuilder<DataDbContext>();
optionsBuilder.UseSqlServer(configuration.GetConnectionString("conn"));

using var dbContext = new DataDbContext(optionsBuilder.Options);

try
{
    string? message;
    while ((message = Console.ReadLine()) != null)
    {
        var response = await producer.ProduceAsync("send-message",
            new Message<Null, string>
            {
                Value = message
            }
        );

        Console.WriteLine($"Message sent: {response.Value}");

        // Save to SQL Server
        var entity = new Message { Chat = message };
        dbContext.Message.Add(entity);
        await dbContext.SaveChangesAsync();
    }
}
catch (ProduceException<Null, string> exc)
{
    Console.WriteLine($"Error producing message: {exc.Error.Reason}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex}");
}
