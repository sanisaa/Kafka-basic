

namespace Api
{
    public interface IMessagePublisher
    {
        Task ProduceAsync(string message);
    }
}
