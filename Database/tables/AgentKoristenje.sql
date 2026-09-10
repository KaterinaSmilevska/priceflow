CREATE TABLE [dbo].[AgentKoristenje]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[KorisnikId] INT NOT NULL,
	[Datum] DATE NOT NULL,
	[BrojPoraki] INT NOT NULL DEFAULT 0,

	CONSTRAINT pk_AgentKoristenje PRIMARY KEY (Id),
	CONSTRAINT un_AgentKoristenje_KorisnikId_Datum UNIQUE (KorisnikId, Datum),
	CONSTRAINT fk_AgentKoristenje_Korisnici FOREIGN KEY (KorisnikId) REFERENCES Korisnici(Id) ON DELETE CASCADE
)
