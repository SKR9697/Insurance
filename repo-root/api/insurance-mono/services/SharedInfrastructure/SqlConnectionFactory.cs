using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SharedInfrastructure
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string _conn;
        public SqlConnectionFactory(IConfiguration cfg)
        {
            _conn = cfg.GetConnectionString("Default")!;
        }
        public IDbConnection CreateConnection()
        {
            try
            {
                var c = new SqlConnection(_conn);
                c.Open();
                return c;
            }
            catch (Exception ex)
            {
                var xxxxxxxxxx = ex.InnerException;
                return null;
            }
        }
    }
}
