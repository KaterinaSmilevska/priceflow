CREATE TABLE [dbo].[KorisnikUloga]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[KorisnikId] INT NOT NULL,
	[UlogaId] INT NOT NULL,

	CONSTRAINT pk_KorisnikUloga PRIMARY KEY (Id),
	CONSTRAINT fk_KorisnikUloga_Korisnik FOREIGN KEY (KorisnikId) REFERENCES Korisnik(Id),
	CONSTRAINT fk_KorisnikUloga_Uloga FOREIGN KEY (UlogaId) REFERENCES Uloga(Id)
)
