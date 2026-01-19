CREATE TABLE [dbo].[HartiiOdVrednost]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [ISIN] NVARCHAR(12) NOT NULL, 
    [Kod] NVARCHAR(50) NOT NULL, 
    [VkupenBrojAkcii] INT NOT NULL,
    [TipHVId] INT NOT NULL,
    [IzdavachId] INT NOT NULL,
    [DateModified] DATETIME NOT NULL CONSTRAINT df_HartiiOdVrednost_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_HartiiOdVrednost PRIMARY KEY (Id),
    CONSTRAINT un_HartiiOdVrednost_Kod UNIQUE (Kod),
    CONSTRAINT fk_HartiiOdVrednost_TipHV FOREIGN KEY (TipHVId) REFERENCES TipHV(Id) ON DELETE CASCADE,
    CONSTRAINT fk_HartiiOdVrednost_Izdavachi FOREIGN KEY (IzdavachId) REFERENCES Izdavachi(Id) ON DELETE CASCADE
)
