CREATE TABLE [dbo].[Link_KorisniciUlogi]
(
	KorisniciUlogi_HK CHAR(32) NOT NULL,
	Korisnici_HK CHAR(32) NOT NULL,
	Ulogi_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_KorisniciUlogi PRIMARY KEY (KorisniciUlogi_HK),
	CONSTRAINT fk_Link_KorisniciUlogi_Hub_Korisnici FOREIGN KEY (Korisnici_HK) REFERENCES Hub_Korisnici(Korisnici_HK),
	CONSTRAINT fk_Link_KorisniciUlogi_Hub_Ulogi FOREIGN KEY (Ulogi_HK) REFERENCES Hub_Ulogi(Ulogi_HK)
)
