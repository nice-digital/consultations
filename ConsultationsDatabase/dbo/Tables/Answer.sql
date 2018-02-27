CREATE TABLE [dbo].[Answer] (
    [AnswerID]         INT              IDENTITY (1, 1) NOT NULL,
    [QuestionID]       INT              NOT NULL,
    [UserID]           UNIQUEIDENTIFIER NOT NULL,
    [AnswerText]       NVARCHAR (MAX)   NULL,
    [AnswerBoolean]    BIT              NULL,
    [CreatedDate]      DATETIME2 (7)    CONSTRAINT [DF_Answer_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [LastModifiedDate] DATETIME2 (7)    CONSTRAINT [DF_Answer_LastModifiedDate] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Answer] PRIMARY KEY CLUSTERED ([AnswerID] ASC),
    CONSTRAINT [FK_Answer_Question] FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[Question] ([QuestionID])
);

