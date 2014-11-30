CREATE TABLE [dbo].[Items]
(
	[ItemId] VARCHAR(20) NOT NULL, 
    [Title] NVARCHAR(100) NOT NULL, 
    [Body] NVARCHAR(MAX) NOT NULL, 
    [UserId] NVARCHAR(128) NOT NULL, 
    [IsPrivate] BIT NOT NULL DEFAULT 0, 
    [CreateDateTimeUtc] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), 
    [UpdateDateTimeUtc] DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(), 
    CONSTRAINT [PK_Items] PRIMARY KEY ([ItemId]), 
    CONSTRAINT [FK_Items_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId])
)

GO

CREATE INDEX [IX_Items_UserId] ON [dbo].[Items] ([UserId])
GO
