using Api.Model;
using Microsoft.EntityFrameworkCore;

public class DataDbContext : DbContext
{
    public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
    {
    }

    public DbSet<Message> Message { get; set; }
    public DbSet<consumer_messages> consumer_messages { get; set; }
}
