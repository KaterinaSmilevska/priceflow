CREATE TABLE [dbo].[Portfolija]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Ime] NVARCHAR(50) NOT NULL, 
    [Opis] NVARCHAR(100) NULL,
    [KorisnikId] INT NOT NULL,
    [DateModified] DATETIME NOT NULL CONSTRAINT df_Portfolija_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_Portfolija PRIMARY KEY (Id),
    CONSTRAINT un_Portfolija_KorisnikId_Ime UNIQUE (KorisnikId, Ime),
    CONSTRAINT fk_Portfolija_Korisnici FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE CASCADE
)
