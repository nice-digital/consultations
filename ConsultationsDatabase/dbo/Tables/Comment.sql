CREATE TABLE [dbo].[Comment] (
    [CommentID]        INT              IDENTITY (1, 1) NOT NULL,
    [LocationID]       INT              NOT NULL,
    [UserID]           UNIQUEIDENTIFIER NOT NULL,
    [CommentText]      NVARCHAR (MAX)   NOT NULL,
    [CreatedDate]      DATETIME2 (7)    CONSTRAINT [DF_Comment_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [LastModifiedDate] DATETIME2 (7)    CONSTRAINT [DF_Comment_LastModifiedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Comment] PRIMARY KEY CLUSTERED ([CommentID] ASC),
    CONSTRAINT [FK_Comment_Location] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Location] ([LocationID])
);

