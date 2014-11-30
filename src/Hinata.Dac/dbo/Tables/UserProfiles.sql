CREATE TABLE [dbo].[UserProfiles]
(
	[UserId] NVARCHAR(128) NOT NULL, 
    [Description] NVARCHAR(200) NULL, 
    CONSTRAINT [PK_UserProfiles] PRIMARY KEY ([UserId]), 
    CONSTRAINT [FK_UserProfiles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
)
