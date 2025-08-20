CREATE TABLE [dbo].[HartiiOdVrednost]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [ISIN] NVARCHAR(12) NOT NULL, 
    [Kod] NVARCHAR(50) NOT NULL, 
    [VkupenBrojAkcii] INT NOT NULL,
    [TipHVId] INT NOT NULL,
    [IzdavachId] INT NOT NULL,

    CONSTRAINT pk_HartiiOdVrednost PRIMARY KEY (Id),
    CONSTRAINT un_Kod UNIQUE (Kod),
    CONSTRAINT fk_HartiiOdVrednost_TipHV FOREIGN KEY (TipHVId) REFERENCES TipHV(Id),
    CONSTRAINT fk_HartiiOdVrednost_Izdavach FOREIGN KEY (IzdavachId) REFERENCES Izdavach(Id)
)
