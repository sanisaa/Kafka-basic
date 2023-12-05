using System;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;

public class consumer_messages
{
    public int Id { get; set; }
    public string? Chat { get; set; }
}

public class MyDbContext : DbContext
{
    public DbSet<consumer_messages>? consumer_messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-SANISHA;Initial Catalog=Messages;Integrated Security=True;TrustServerCertificate=True;");
    }
}

public class Consumer
{
    public static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: Consumer <consumer-number>");
            return;
        }

        int consumerNumber;
        if (!int.TryParse(args[0], out consumerNumber))
        {
            Console.WriteLine("Invalid consumer number");
            return;
        }

        ReadMessage(consumerNumber);
    }

    public static void ReadMessage(int consumerNumber)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            GroupId = $"message-consumer-group-{consumerNumber}", 
            EnableAutoCommit = false,
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("send-message"); // Subscribe to the topic

        try
        {
            while (true)
            {
                var consumeResult = consumer.Consume();
                Console.WriteLine($"Consumer {consumerNumber} - Assigned Partition: {consumeResult.Partition}, Message: {consumeResult.Message.Value}");
                SendDataToDatabase(consumeResult.Message.Value);

                consumer.Commit(consumeResult);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            consumer.Close();
        }
    }

    public static void SendDataToDatabase(string messageContent)
    {
        using var dbContext = new MyDbContext();
        if (dbContext.consumer_messages != null)
        {
            var consumerEntity = new consumer_messages { Chat = messageContent };
            dbContext.consumer_messages.Add(consumerEntity);
            dbContext.SaveChanges();
        }
    }
}
