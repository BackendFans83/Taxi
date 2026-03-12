namespace AuthService.DTOs;

public class CreateUserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Role { get; set; }
    
    public CreateUserDto(int id, string name, string role)
    {
        Id = id;
        Name = name;
        Role = role;
    }
}