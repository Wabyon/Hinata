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
    public class DraftRepository : IDraftRepository
    {
        private readonly string _connectionString;

        public DraftRepository(string connectionString)
        {
            if (connectionString == null) throw new ArgumentNullException("connectionString");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentException("connectionString is empty", "connectionString");
            _connectionString = connectionString;
        }

        public async Task<Draft> FindByIdAsync(string id)
        {
            const string sql = @"
IF NOT EXISTS (SELECT * FROM Drafts WHERE ItemId = @ItemId) AND EXISTS (SELECT * FROM Items WHERE ItemId = @ItemId)
BEGIN
    INSERT INTO Drafts (
         [ItemId]
        ,[Title]
        ,[Body]
        ,[UserId]
        ,[RegisterDateTimeUtc]
    )
    SELECT
         [ItemId]
        ,[Title]
        ,[Body]
        ,[UserId]
        ,[RegisterDateTimeUtc] = SYSUTCDATETIME()
    FROM Items
    WHERE
        ItemId = @ItemId

    INSERT INTO DraftTags (
         [ItemId]
        ,[OrderNo]
        ,[TagName]
        ,[Version]
    )
    SELECT
         [ItemId]
        ,[OrderNo]
        ,[TagName]
        ,[Version]
    FROM ItemTags
    WHERE
        ItemId = @ItemId
END

SELECT
     [Id] = Drafts.ItemId
    ,[Title] = Drafts.Title
    ,[User] = Users.[User]
    ,[Body] = Drafts.Body
    ,[RegisterDateTimeUtc] = Drafts.RegisterDateTimeUtc
    ,[IsContributed] = CONVERT(BIT,IIF(Items.ItemId IS NULL, 0, 1))
    ,[Tags] = DraftTags.Tags
FROM Drafts
OUTER APPLY (
    SELECT
        [User] = 
              '{""Id"": ""' + UserId + '"", '
            + '""UserName"": ""' + UserName + '""}'
    FROM Users
    WHERE
        Users.UserId = Drafts.UserId
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
                FROM DraftTags
                WHERE
                    Drafts.ItemId = DraftTags.ItemId
            ) Sub FOR XML PATH('')
    ), '} {', '}, {') + ']'
) DraftTags
LEFT OUTER JOIN Items
ON  Drafts.ItemId = Items.ItemId
WHERE
    Drafts.ItemId = @ItemId
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                var result = (await cn.QueryAsync(sql, new { ItemId = id })).FirstOrDefault();
                return result == null ? null : DynamicToDraft(result);
            }
        }

        public async Task<IEnumerable<Draft>> GetByUserIdAsync(string userId)
        {
            const string sql = @"
SELECT
     [Id] = Drafts.ItemId
    ,[Title] = Drafts.Title
    ,[User] = Users.[User]
    ,[Body] = Drafts.Body
    ,[RegisterDateTimeUtc] = Drafts.RegisterDateTimeUtc
    ,[IsContributed] = CONVERT(BIT,IIF(Items.ItemId IS NULL, 0, 1))
    ,[Tags] = DraftTags.Tags
FROM Drafts
OUTER APPLY (
    SELECT
        [User] = 
              '{""Id"": ""' + UserId + '"", '
            + '""UserName"": ""' + UserName + '""}'
    FROM Users
    WHERE
        Users.UserId = Drafts.UserId
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
                FROM DraftTags
                WHERE
                    Drafts.ItemId = DraftTags.ItemId
            ) Sub FOR XML PATH('')
    ), '} {', '}, {') + ']'
) DraftTags
LEFT OUTER JOIN Items
ON  Drafts.ItemId = Items.ItemId
WHERE
    Drafts.UserId = @UserId
ORDER BY
     Drafts.RegisterDateTimeUtc DESC
";

            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                return (await cn.QueryAsync(sql, new { UserId = userId }))
                    .Select(x => DynamicToDraft(x))
                    .Cast<Draft>();
            }
        }

        public async Task SaveAsync(Draft draft)
        {
            const string sqlDraft = @"
MERGE INTO Drafts Main
USING (
SELECT
     [ItemId] = @ItemId
    ,[Title] = @Title
    ,[Body] = @Body
    ,[UserId] = @UserId
    ,[RegisterDateTimeUtc] = @RegisterDateTimeUtc
) Sub
ON Main.ItemId = Sub.ItemId
WHEN MATCHED THEN
UPDATE
SET  Main.Title = Sub.Title
    ,Main.Body = Sub.Body
    ,Main.UserId = Sub.UserId
    ,Main.RegisterDateTimeUtc = Sub.RegisterDateTimeUtc
WHEN NOT MATCHED THEN
INSERT VALUES (
     Sub.ItemId
    ,Sub.Title
    ,Sub.Body
    ,Sub.UserId
    ,Sub.RegisterDateTimeUtc
);
";
            const string sqlDraftTags = @"
INSERT INTO DraftTags (
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
                        await cn.ExecuteAsync(@"DELETE FROM Drafts WHERE ItemId = @ItemId", new { ItemId = draft.Id }, tr);

                        await
                            cn.ExecuteAsync(sqlDraft,
                                new { ItemId = draft.Id, draft.Title, draft.Body, UserId = draft.User.Id, draft.RegisterDateTimeUtc }, tr);

                        foreach (var tag in draft.Tags)
                        {
                            await
                                cn.ExecuteAsync(sqlDraftTags,
                                    new { ItemId = draft.Id, TagName = tag.Name, tag.Version, tag.OrderNo }, tr);
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

        public async Task DeleteAsync(Draft draft)
        {
            using (var cn = new SqlConnection(_connectionString))
            {
                await cn.OpenAsync();

                await cn.ExecuteAsync(@"DELETE FROM Drafts WHERE ItemId = @ItemId", new { ItemId = draft.Id });
            }
        }

        private static Draft DynamicToDraft(dynamic draft)
        {
            if (draft == null) throw new ArgumentNullException("draft");

            var entity = new Draft
            {
                Id = draft.Id,
                Body = draft.Body,
                Title = draft.Title,
                RegisterDateTimeUtc = draft.RegisterDateTimeUtc,
                IsContributed = draft.IsContributed,
                User = JsonConvert.DeserializeObject<User>(draft.User),
            };

            var jTags = draft.Tags;
            if (jTags == null) return entity;

            foreach (var tag in JsonConvert.DeserializeObject<TagDetailCollection>(jTags))
            {
                entity.Tags.Add(tag);
            }

            return entity;
        }
    }
}
