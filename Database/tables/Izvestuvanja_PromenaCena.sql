CREATE TABLE [dbo].[Izvestuvanja_PromenaCena]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [KorisnikId] INT NOT NULL,
    [HVId] INT NOT NULL,
    [ProcentPromena] DECIMAL(18, 2) NOT NULL,
    [DatumTrguvanje] DATETIME NOT NULL,
    [Poraka] NVARCHAR(500) NOT NULL,
    [Procitano] BIT NOT NULL CONSTRAINT df_Izvestuvanja_PromenaCena_Procitano DEFAULT (0),
    [DateModified] DATETIME NOT NULL CONSTRAINT df_Izvestuvanja_PromenaCena_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_Izvestuvanja_PromenaCena PRIMARY KEY (Id),
    CONSTRAINT fk_Izvestuvanja_PromenaCena_Korisnici FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE CASCADE,
    CONSTRAINT fk__Izvestuvanja_PromenaCena_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id) ON DELETE CASCADE
)
