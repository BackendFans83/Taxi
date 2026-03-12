namespace AuthService.DTOs;

public class KafkaEvent<T>
{
    public string Event { get; set; }
    public T Data { get; set; }

    public KafkaEvent(string topic, T data)
    {
        Event = topic;
        Data = data;
    }
}