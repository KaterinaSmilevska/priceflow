CREATE TABLE [dbo].[Portfolio]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Ime] NVARCHAR(50) NOT NULL, 
    [Opis] NVARCHAR(100) NULL,
    [KorisnikId] INT NOT NULL,

    CONSTRAINT pk_Portfolio PRIMARY KEY (Id),
    CONSTRAINT un_Portfolio_KorisnikId_Ime UNIQUE (KorisnikId, Ime),
    CONSTRAINT fk_Portfolio_Korisnik FOREIGN KEY (KorisnikId) REFERENCES Korisnik(Id)
)
