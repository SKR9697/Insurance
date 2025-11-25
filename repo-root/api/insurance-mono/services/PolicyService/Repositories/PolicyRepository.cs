using Dapper;
using PolicyService.Models;
using SharedInfrastructure;

namespace PolicyService.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly IDbConnectionFactory _factory;
        public PolicyRepository(IDbConnectionFactory factory) { _factory = factory; }

        public async Task<IEnumerable<Policy>> GetAllAsync()
        {
            try
            {
                using var db = _factory.CreateConnection();
                return await db.QueryAsync<Policy>("SELECT * FROM dbo.Policies ORDER BY Id DESC");
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<Policy?> GetAsync(int id)
        {
            using var db = _factory.CreateConnection();
            return await db.QuerySingleOrDefaultAsync<Policy>(
              "SELECT * FROM dbo.Policies WHERE Id = @Id", new { Id = id });
        }

        public async Task<int> CreateAsync(Policy p)
        {
            using var db = _factory.CreateConnection();
            var sql = @"INSERT INTO dbo.Policies
            (PolicyNumber, Title, Status, PremiumAmount, StartDate, EndDate, PolicyConfigId, OwnerUserId, CreatedAt)
            VALUES (@PolicyNumber, @Title, @Status, @PremiumAmount, @StartDate, @EndDate, @PolicyConfigId, @OwnerUserId, @CreatedAt);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";
            return await db.ExecuteScalarAsync<int>(sql, p);
        }

        public async Task<bool> UpdateAsync(Policy p)
        {
            using var db = _factory.CreateConnection();
            var sql = @"UPDATE dbo.Policies SET
            Title=@Title, Status=@Status, PremiumAmount=@PremiumAmount,
            StartDate=@StartDate, EndDate=@EndDate, PolicyConfigId=@PolicyConfigId
            WHERE Id=@Id";
            var rows = await db.ExecuteAsync(sql, p);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = _factory.CreateConnection();
            var rows = await db.ExecuteAsync("DELETE FROM dbo.Policies WHERE Id=@Id", new { Id = id });
            return rows > 0;
        }
    }
}
