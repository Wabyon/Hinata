CREATE TABLE [dbo].[ItemLogs]
(
	[ItemId] VARCHAR(20) NOT NULL,
	[LogDateTimeUtc] DATETIME2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
	[Action] VARCHAR(10) NOT NULL,
    [Title] NVARCHAR(100) NOT NULL,
    [Body] NVARCHAR(MAX) NOT NULL,
    [UserId] NVARCHAR(128) NOT NULL,
    [IsPrivate] BIT NOT NULL,
    [CreateDateTimeUtc] DATETIME2 NOT NULL,
    [UpdateDateTimeUtc] DATETIME2 NOT NULL, 
    CONSTRAINT [PK_ItemLogs] PRIMARY KEY ([ItemId], [LogDateTimeUtc]),
)
