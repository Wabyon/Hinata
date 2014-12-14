using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hinata.Identity;
using Hinata.Identity.Repositories;

namespace Hinata.Data.SqlServer
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly string _connectionString;

        public ApplicationUserRepository(string connectionString)
        {
            _connectionString = connectionString;

            SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
            SqlMapper.AddTypeMap(typeof(DateTime?), DbType.DateTime2);
        }

        public async Task CreateAsync(ApplicationUser user)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
INSERT INTO [dbo].[Users] (
     [UserId]
    ,[UserName]
    ,[Email]
    ,[EmailConfirmed]
    ,[PasswordHash]
    ,[LockoutEndDateUtc]
    ,[LockoutEnabled]
    ,[AccessFailedCount]
)
VALUES (
     @Id
    ,@UserName
    ,@Email
    ,@EmailConfirmed
    ,@PasswordHash
    ,@LockoutEndDateUtc
    ,@LockoutEnabled
    ,@AccessFailedCount
)
";
                await
                    cn.ExecuteAsync(sql, user);
            }
        }

        public async Task UpdateAsync(ApplicationUser user)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
UPDATE [dbo].[Users]
SET  [UserName] = @UserName
    ,[Email] = @Email
    ,[EmailConfirmed] = @EmailConfirmed
    ,[PasswordHash] = @PasswordHash
    ,[LockoutEndDateUtc] = @LockoutEndDateUtc
    ,[LockoutEnabled] = @LockoutEnabled
    ,[AccessFailedCount] = @AccessFailedCount
WHERE
    [UserId] = @Id
";

                await cn.ExecuteAsync(sql, user);
            }
        }

        public async Task DeleteAsync(ApplicationUser user)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
DELETE FROM [dbo].[Users]
WHERE [UserId] = @Id
";

                await cn.ExecuteAsync(sql, new { UserId = user.Id });
            }
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
SELECT
     [Id] = [UserId]
    ,[UserName]
    ,[Email]
    ,[EmailConfirmed]
    ,[PasswordHash]
    ,[LockoutEndDateUtc]
    ,[LockoutEnabled]
    ,[AccessFailedCount]
FROM [dbo].[Users]
WITH (NOLOCK)
WHERE
    [UserId] = @UserId
";
                var users = await cn.QueryAsync<ApplicationUser>(sql, new { UserId = userId });

                return users.FirstOrDefault();
            }
        }

        public async Task<ApplicationUser> FindByNameAsync(string userName)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
SELECT
     [Id] = [UserId]
    ,[UserName]
    ,[Email]
    ,[EmailConfirmed]
    ,[PasswordHash]
    ,[LockoutEndDateUtc]
    ,[LockoutEnabled]
    ,[AccessFailedCount]
FROM [dbo].[Users]
WITH (NOLOCK)
WHERE
    [UserName] = @UserName
";
                var users = await cn.QueryAsync<ApplicationUser>(sql, new { UserName = userName });

                return users.FirstOrDefault();
            }
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
SELECT
     [Id] = [UserId]
    ,[UserName]
    ,[Email]
    ,[EmailConfirmed]
    ,[PasswordHash]
    ,[LockoutEndDateUtc]
    ,[LockoutEnabled]
    ,[AccessFailedCount]
FROM [dbo].[Users]
WITH (NOLOCK)
WHERE
    [Email] = @Email
";
                var users = await cn.QueryAsync<ApplicationUser>(sql, new { Email = email });

                return users.FirstOrDefault();
            }
        }
    }
}
