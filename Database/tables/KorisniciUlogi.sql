CREATE TABLE [dbo].[KorisniciUlogi]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[KorisnikId] INT NOT NULL,
	[UlogaId] INT NOT NULL,
	[DateModified] DATETIME NOT NULL CONSTRAINT df_KorisniciUlogi_DateModified DEFAULT (SYSUTCDATETIME()),

	CONSTRAINT pk_KorisniciUlogi PRIMARY KEY (Id),
	CONSTRAINT fk_KorisniciUlogi_Korisnici FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id),
	CONSTRAINT fk_KorisniciUlogi_Ulogi FOREIGN KEY (UlogaId) REFERENCES Ulogi(Id)
)
