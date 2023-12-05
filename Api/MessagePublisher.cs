using Confluent.Kafka;


namespace Api
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IProducer<Null, string> producer;

        public MessagePublisher(IProducer<Null, string> producer)
        {
            this.producer = producer;
        }

        public async Task ProduceAsync(string message)
        {
            await producer.ProduceAsync("send-message",
                new Message<Null, string>
                {
                    Value = message
                });
        }
    }
}
