CREATE TABLE [dbo].[Sat_Korisnici]
(
	Korisnici_HK CHAR(32) NOT NULL,
	Ime NVARCHAR(100) NOT NULL,
	Email NVARCHAR(100) NOT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_Korisnici PRIMARY KEY (Korisnici_HK, LoadDate),
	CONSTRAINT fk_Sat_Korisnici_Hub_Korisnici FOREIGN KEY (Korisnici_HK) REFERENCES Hub_Korisnici(Korisnici_HK)
)
