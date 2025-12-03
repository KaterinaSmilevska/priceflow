CREATE NONCLUSTERED INDEX [IX_Link_KorisniciUlogi_Korisnici_HK]
	ON [dbo].[Link_KorisniciUlogi]
	(Korisnici_HK)
WHERE Korisnici_HK IS NOT NULL
