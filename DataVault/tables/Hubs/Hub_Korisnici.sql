CREATE TABLE [dbo].[Hub_Korisnici]
(
	Korisnici_HK CHAR(32) NOT NULL,
	Username NVARCHAR(100) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_Korisnici PRIMARY KEY (Korisnici_HK),
	CONSTRAINT un_Hub_Korisnici_Username UNIQUE (Username)
)
