using Dapper;
using SharedInfrastructure;
using UserService.Models;

namespace UserService.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _factory;
        public UserRepository(IDbConnectionFactory factory) { _factory = factory; }

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var db = _factory.CreateConnection();
            return await db.QuerySingleOrDefaultAsync<User>(
              "SELECT Id, Email, PasswordHash, Role, CreatedAt FROM dbo.Users WHERE Email = @Email",
              new { Email = email }
            );
        }

        public async Task<int> CreateAsync(string email, string passwordHash, string role = "User")
        {
            using var db = _factory.CreateConnection();
            var sql = @"INSERT INTO dbo.Users (Email, PasswordHash, Role)
                VALUES (@Email, @PasswordHash, @Role);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await db.ExecuteScalarAsync<int>(sql, new { Email = email, PasswordHash = passwordHash, Role = role });
        }
    }
}
