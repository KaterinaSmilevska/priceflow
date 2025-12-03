CREATE TABLE [dbo].[Link_Portfolija_Korisnici]
(
	Portfolija_Korisnici_HK CHAR(32) NOT NULL,
	Portfolija_HK CHAR(32) NOT NULL,
	Korisnici_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_Portfolija_Korisnici PRIMARY KEY (Portfolija_Korisnici_HK),
	CONSTRAINT fk_Link_Portfolija_Korisnici_Hub_Portfolija FOREIGN KEY (Portfolija_HK) REFERENCES Hub_Portfolija(Portfolija_HK),
	CONSTRAINT fk_Link_Portfolija_Korisnici_Hub_Korisnici FOREIGN KEY (Korisnici_HK) REFERENCES Hub_Korisnici(Korisnici_HK)
)
