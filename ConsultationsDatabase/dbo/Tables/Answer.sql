CREATE TABLE [dbo].[Answer] (
    [AnswerID]         INT              IDENTITY (1, 1) NOT NULL,
    [QuestionID]       INT              NOT NULL,
    [CreatedByUserID]           UNIQUEIDENTIFIER NOT NULL,
    [AnswerText]       NVARCHAR (MAX)   NULL,
    [AnswerBoolean]    BIT              NULL,
    [CreatedDate]      DATETIME2 (7)    CONSTRAINT [DF_Answer_CreatedDate] DEFAULT (getdate()) NOT NULL,
    [LastModifiedDate] DATETIME2 (7)    CONSTRAINT [DF_Answer_LastModifiedDate] DEFAULT (getdate()) NOT NULL,
    [LastModifiedByUserID] UNIQUEIDENTIFIER NOT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_Answer] PRIMARY KEY CLUSTERED ([AnswerID] ASC),
    CONSTRAINT [FK_Answer_Question] FOREIGN KEY ([QuestionID]) REFERENCES [dbo].[Question] ([QuestionID])
);

