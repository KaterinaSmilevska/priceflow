CREATE TABLE [dbo].[HV_PromenaCena]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [KorisnikId] INT NOT NULL,
    [HVId] INT NOT NULL,
    [DolnaGranica] DECIMAL(18,2) NOT NULL,
    [GornaGranica] DECIMAL(18, 2) NOT NULL,
    [DateModified] DATETIME NOT NULL CONSTRAINT df_HV_PromenaCena_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_HV_PromenaCena PRIMARY KEY (Id),
    CONSTRAINT un_HV_PromenaCena_Korisnici_HartiiOdVrednost UNIQUE (KorisnikId, HVId),
    CONSTRAINT fk_HV_PromenaCena_Korisnici FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE CASCADE,
    CONSTRAINT fk_HV_PromenaCena_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id) ON DELETE CASCADE,
    CONSTRAINT ck_HV_PromenaCena_Granici CHECK (DolnaGranica < GornaGranica)
)
