namespace ServiceBus
{
    public interface IMessageBus
    {
        Task PublishMessage(string connection, object message, string topic_queue_name);
    }
}
