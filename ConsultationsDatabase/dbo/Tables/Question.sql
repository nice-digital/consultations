CREATE TABLE [dbo].[Question] (
    [QuestionID]     INT            IDENTITY (1, 1) NOT NULL,
    [LocationID]     INT            NOT NULL,
    [QuestionText]   NVARCHAR (MAX) NOT NULL,
    [QuestionTypeID] INT            NOT NULL,
    [QuestionOrder]  TINYINT        NULL,
    CONSTRAINT [PK_Question] PRIMARY KEY CLUSTERED ([QuestionID] ASC),
    CONSTRAINT [FK_Question_Location] FOREIGN KEY ([LocationID]) REFERENCES [dbo].[Location] ([LocationID]),
    CONSTRAINT [FK_Question_QuestionType] FOREIGN KEY ([QuestionTypeID]) REFERENCES [dbo].[QuestionType] ([QuestionTypeID])
);

