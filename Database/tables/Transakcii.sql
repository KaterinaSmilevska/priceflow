CREATE TABLE [dbo].[Transakcii]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [PortfolioId] INT NOT NULL, 
    [KolicinaAkcii] INT NOT NULL, 
    [EdinecnaCenaAkcija] INT NOT NULL, 
    [Datum] DATE NOT NULL, 
    [Iznos] DECIMAL NOT NULL, 
    [BerzanskaProvizija] DECIMAL(18, 2) NOT NULL, 
    [BrokerskaProvizija] DECIMAL(18, 2) NOT NULL, 
    [CDHVProvizija] DECIMAL(18, 2) NOT NULL, 
    [TipTransakcija] NVARCHAR(20) NOT NULL, 
    [Realna] NVARCHAR(2) NOT NULL,
    [HVId] INT NOT NULL, 

    CONSTRAINT pk_Transakcii PRIMARY KEY (Id),
    CONSTRAINT fk_Transakcii_Portfolio FOREIGN KEY (PortfolioId) REFERENCES Portfolio(Id),
    CONSTRAINT fk_Transakcii_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id)
)
