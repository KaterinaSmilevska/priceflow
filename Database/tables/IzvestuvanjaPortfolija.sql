CREATE TABLE [dbo].[IzvestuvanjaPortfolija]
(
	[Id] INT NOT NULL,
	[PortfolioId] INT NOT NULL,
	[Ovozmozeno] BIT NOT NULL,
	[Frekvencija] NVARCHAR(10) NOT NULL CONSTRAINT df_IzvestuvanjaPortfolija_Frekvencija DEFAULT 'Weekly',
	[PoslednoIsprateno] DATETIME NULL,

	CONSTRAINT pk_IzvestuvanjaPortfolija PRIMARY KEY (Id),
	CONSTRAINT un_IzvestuvanjaPortfolija_PortfolioId UNIQUE (PortfolioId),
	CONSTRAINT fk_IzvestuvanjaPortfolija_Portfolija FOREIGN KEY (PortfolioId) REFERENCES Portfolija(Id) ON DELETE CASCADE
)
