using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hinata.Models;
using Hinata.Repositories;

namespace Hinata.Data.SqlServer
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is empty", "connectionString");
            _connectionString = connectionString;
        }

        public async Task<User> FindByIdAsync(string id)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
SELECT
     Id = UserId
    ,UserName
FROM Users
WHERE
    UserId = @UserId
";
                return (await cn.QueryAsync<User>(sql, new {UserId = id})).FirstOrDefault();
            }
        }

        public async Task<User> FindByNameAsync(string name)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
SELECT
     Id = UserId
    ,UserName
FROM Users
WHERE
    UserName = @UserName
";
                return (await cn.QueryAsync<User>(sql, new {UserName = name})).FirstOrDefault();
            }
        }
    }
}