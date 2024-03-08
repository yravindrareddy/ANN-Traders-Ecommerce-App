using System.Text.Json;
using System.Text;
using Azure.Messaging.ServiceBus;

namespace ServiceBus
{
    public class MessageBus : IMessageBus
    {
        public async Task PublishMessage(string connection, object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connection);
            var sender = client.CreateSender(topic_queue_name);

            var jsonMessage = JsonSerializer.Serialize(message);
            ServiceBusMessage svcMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString()
            };

            await sender.SendMessageAsync(svcMessage);
            await client.DisposeAsync();
        }
    }
}