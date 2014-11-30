CREATE TABLE [dbo].[DraftTags]
(
	[ItemId] VARCHAR(20) NOT NULL, 
    [OrderNo] INT NOT NULL, 
    [TagName] NVARCHAR(20) NOT NULL, 
    [Version] NVARCHAR(10) NULL, 
    CONSTRAINT [PK_DraftTags] PRIMARY KEY ([ItemId], [OrderNo]), 
    CONSTRAINT [FK_DraftTags_Drafts] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Drafts]([ItemId]) ON DELETE CASCADE,
)
