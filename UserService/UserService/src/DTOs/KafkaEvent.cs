namespace UserService.DTOs;

public class KafkaEvent<T>
{
    public string Event { get; set; }
    public T Data { get; set; }
}