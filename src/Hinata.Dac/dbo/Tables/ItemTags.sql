CREATE TABLE [dbo].[ItemTags]
(
	[ItemId] VARCHAR(20) NOT NULL, 
    [OrderNo] INT NOT NULL, 
    [TagName] NVARCHAR(20) NOT NULL, 
    [Version] NVARCHAR(10) NULL , 
    CONSTRAINT [PK_ItemTags] PRIMARY KEY ([ItemId], [OrderNo]), 
    CONSTRAINT [FK_ItemTags_Items] FOREIGN KEY ([ItemId]) REFERENCES [dbo].[Items]([ItemId]) ON DELETE CASCADE,
)
