CREATE TABLE [dbo].[Question] (
    [QuestionID]     INT            IDENTITY (1, 1) NOT NULL,
    [LocationID]     INT            NOT NULL,
    [QuestionText]   NVARCHAR (MAX) NOT NULL,
    [QuestionTypeID] INT            NOT NULL,
    [QuestionOrder]  TINYINT        NULL,
    [CreatedByUserID] UNIQUEIDENTIFIER NOT NULL, 
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETDATE(), 
    [LastModifiedByUserID] UNIQUEIDENTIFIER NOT NULL, 
    [LastModifiedDate] DATETIME2 NOT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([QuestionID] ASC),
    CONSTRAINT [FK_Question_Location] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Location] ([LocationID]),
    CONSTRAINT [FK_Question_QuestionType] FOREIGN KEY ([QuestionTypeID]) REFERENCES [dbo].[QuestionType] ([QuestionTypeID])
);

