namespace UserService.Models
{
    public record User(int Id, string Email, string PasswordHash, string Role, DateTime CreatedAt);

    public record RegisterDto(string Email, string Password);
    public record LoginDto(string Email, string Password);
}
