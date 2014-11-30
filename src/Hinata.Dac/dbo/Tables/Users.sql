CREATE TABLE [dbo].[Users]
(
	[UserId] NVARCHAR(128) NOT NULL ,
	[UserName] NVARCHAR(20) NOT NULL,
	[Email] NVARCHAR(256) NOT NULL, 
    [EmailConfirmed] BIT NOT NULL DEFAULT 0, 
    [PasswordHash] NVARCHAR(256) NULL, 
    [LockoutEndDateUtc] DATETIME2 NULL, 
    [LockoutEnabled] BIT NOT NULL DEFAULT 0, 
    [AccessFailedCount] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
)
GO


CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users] ([Email])
GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [dbo].[Users] ([UserName])
GO
