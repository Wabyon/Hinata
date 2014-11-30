using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Hinata.Models;
using Hinata.Repositories;
using Newtonsoft.Json;

namespace Hinata.Data.SqlServer
{
    public class ItemRepository : IItemRepository
    {
        private readonly string _connectionString;

        public ItemRepository(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is empty", "connectionString");
            _connectionString = connectionString;
        }

        public async Task<Item> FindByIdAsync(string id)
        {
            const string sql = SelectSql + @"
WHERE Items.ItemId = @ItemId
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                var result = (await cn.QueryAsync(sql, new { ItemId = id })).FirstOrDefault();

                return result == null ? null : DynamicToItem(result);
            }
        }

        public async Task<IEnumerable<Item>> GetPublicAsync(string userId, int skip, int take)
        {
            const string sql = SelectSql + @"
WHERE
    Items.IsPrivate = 0
ORDER BY
     Items.UpdateDateTimeUtc DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                return
                    (await cn.QueryAsync(sql, new { UserId = userId, Skip = skip, Take = take }))
                        .Select(x => DynamicToItem(x))
                        .Cast<Item>();
            }
        }

        public async Task<IEnumerable<Item>> GetFollowingAsync(string userId, int skip, int take)
        {
            const string sql = SelectSql + @"
WHERE
    Items.IsPrivate = 0
AND Items.UserId <> @UserId
ORDER BY
     Items.UpdateDateTimeUtc DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                return
                    (await cn.QueryAsync(sql, new { UserId = userId, Skip = skip, Take = take }))
                        .Select(x => DynamicToItem(x))
                        .Cast<Item>();
            }
        }

        public async Task<IEnumerable<Item>> GetUserPublicAsync(string userId, int skip, int take)
        {
            const string sql = SelectSql + @"
WHERE
    Items.IsPrivate = 0
AND Items.UserId = @UserId
ORDER BY
     Items.UpdateDateTimeUtc DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                return
                    (await cn.QueryAsync(sql, new { UserId = userId, Skip = skip, Take = take }))
                        .Select(x => DynamicToItem(x))
                        .Cast<Item>();
            }
        }

        public async Task<IEnumerable<Item>> GetUserPrivateAsync(string userId, int skip, int take)
        {
            const string sql = SelectSql + @"
WHERE
    Items.IsPrivate = 1
AND Items.UserId = @UserId
ORDER BY
     Items.UpdateDateTimeUtc DESC
OFFSET @Skip ROWS
FETCH NEXT @Take ROWS ONLY
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                return
                    (await cn.QueryAsync(sql, new { UserId = userId, Skip = skip, Take = take }))
                        .Select(x => DynamicToItem(x))
                        .Cast<Item>();
            }
        }

        public async Task SaveAsync(Item item)
        {
            const string sql = @"
MERGE INTO Items Main
USING (
SELECT
     [ItemId] = @ItemId
    ,[Title] = @Title
    ,[Body] = @Body
    ,[UserId] = @UserId
    ,[IsPrivate] = @IsPrivate
    ,[RegisterDateTimeUtc] = @RegisterDateTimeUtc
) Sub
ON Main.ItemId = Sub.ItemId
WHEN MATCHED THEN
UPDATE
SET  Main.Title = Sub.Title
    ,Main.Body = Sub.Body
    ,Main.UserId = Sub.UserId
    ,Main.IsPrivate = Sub.IsPrivate
    ,Main.UpdateDateTimeUtc = Sub.RegisterDateTimeUtc
WHEN NOT MATCHED THEN
INSERT VALUES (
     Sub.ItemId
    ,Sub.Title
    ,Sub.Body
    ,Sub.UserId
    ,Sub.IsPrivate
    ,Sub.RegisterDateTimeUtc
    ,Sub.RegisterDateTimeUtc
);
";
            const string sqlTags = @"
INSERT INTO ItemTags (
     ItemId
    ,OrderNo
    ,TagName
    ,Version
) VALUES (
     @ItemId
    ,@OrderNo
    ,@TagName
    ,@Version
);
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                using (var tr = cn.BeginTransaction())
                {
                    try
                    {
                        await cn.ExecuteAsync(@"DELETE FROM Items WHERE ItemId = @ItemId", new { ItemId = item.Id }, tr);

                        await
                            cn.ExecuteAsync(sql,
                                new
                                {
                                    ItemId = item.Id,
                                    item.Title,
                                    item.Body,
                                    UserId = item.User.Id,
                                    item.IsPrivate,
                                    item.RegisterDateTimeUtc
                                }, tr);

                        foreach (var tag in item.Tags)
                        {
                            await
                                cn.ExecuteAsync(sqlTags,
                                    new { ItemId = item.Id, TagName = tag.Name, tag.Version, tag.OrderNo }, tr);
                        }

                        tr.Commit();
                    }
                    catch (Exception)
                    {
                        tr.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task DeleteAsync(Item item)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                const string sql = @"
DELETE FROM Items
WHERE ItemId = @ItemId

DELETE FROM Drafts
WHERE ItemId = @ItemId
";
                await cn.ExecuteAsync(sql, new { Item = item.Id });
            }
        }

        private static Item DynamicToItem(dynamic item)
        {
            if (item == null) throw new ArgumentNullException("item");

            var entity = new Item
            {
                Id = item.Id,
                Body = item.Body,
                Title = item.Title,
                RegisterDateTimeUtc = item.RegisterDateTimeUtc,
                IsPrivate = item.IsPrivate,
                User = JsonConvert.DeserializeObject<User>(item.User),
            };

            var jTags = item.Tags;
            if (jTags == null) return entity;

            foreach (var tag in JsonConvert.DeserializeObject<TagDetailCollection>(jTags))
            {
                entity.Tags.Add(tag);
            }

            return entity;
        }

        #region SelectSql
        private const string SelectSql = @"
SELECT
     [Id] = Items.ItemId
    ,[Title] = Items.Title
    ,[User] = Users.[User]
    ,[Body] = Items.Body
    ,[IsPrivate] = Items.IsPrivate
    ,[RegisterDateTimeUtc] = Items.UpdateDateTimeUtc
    ,[Tags] = ItemTags.Tags
FROM Items
OUTER APPLY (
    SELECT
        [User] = 
              '{""Id"": ""' + UserId + '"", '
            + '""UserName"": ""' + UserName + '""}'
    FROM Users
    WHERE
        Users.UserId = Items.UserId
) Users
OUTER APPLY (
    SELECT
        Tags = '[' + REPLACE((
            SELECT Tag AS [data()]
            FROM (
                SELECT
                     Tag =
                          '{""Name"": ""' + REPLACE(TagName,'""','""""') + '"", '
                        + ISNULL('""Version"": ""' + REPLACE(Version,'""','""""') + '"", ','')
                        + '""OrderNo"": ' + CONVERT(NVARCHAR(3),OrderNo) + '}'
                FROM ItemTags
                WHERE
                    Items.ItemId = ItemTags.ItemId
            ) Sub FOR XML PATH('')
    ), '} {', '}, {') + ']'
) ItemTags
";
        #endregion
    }
}
