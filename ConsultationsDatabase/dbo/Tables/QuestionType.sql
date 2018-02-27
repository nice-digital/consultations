CREATE TABLE [dbo].[QuestionType] (
    [QuestionTypeID]   INT            IDENTITY (1, 1) NOT NULL,
    [Description]      NVARCHAR (100) NOT NULL,
    [HasTextAnswer]    BIT            NOT NULL,
    [HasBooleanAnswer] BIT            NOT NULL,
    CONSTRAINT [PK_QuestionType] PRIMARY KEY CLUSTERED ([QuestionTypeID] ASC)
);

