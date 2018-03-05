CREATE TABLE [dbo].[Location] (
    [LocationID]       INT              IDENTITY (1, 1) NOT NULL,
    [ConsultationID]   INT NOT NULL,
    [DocumentID]       INT NULL,
    [ChapterSlug]      NVARCHAR (MAX)   NULL,
    [SectionSlug]      NVARCHAR (MAX)   NULL,
    [RangeStart]       NVARCHAR (MAX)   NULL,
    [RangeStartOffset] INT              NULL,
    [RangeEnd]         NVARCHAR (MAX)   NULL,
    [RangeEndOffset]   INT              NULL,
    [Quote]            NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Location] PRIMARY KEY CLUSTERED ([LocationID] ASC)
);

