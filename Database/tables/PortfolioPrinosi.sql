CREATE TABLE [dbo].[PortfolioPrinosi]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Datum] DATE NOT NULL, 
    [NetoIznos] DECIMAL NOT NULL, 
    [Danok] DECIMAL(18, 2) NOT NULL,
    [PortfolioId] INT NOT NULL, 
    [HVId] INT NOT NULL, 
    [DateModified] DATETIME NOT NULL CONSTRAINT df_PortfolioPrinosi_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_PortfolioPrinosi PRIMARY KEY (Id),
    CONSTRAINT fk_PortfolioPrinosi_Portfolija FOREIGN KEY (PortfolioId) REFERENCES Portfolija(Id) ON DELETE CASCADE,
    CONSTRAINT fk_PortfolioPrinosi_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id) ON DELETE CASCADE
)
