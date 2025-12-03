CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_KorisniciUlogi]
	ON [dbo].[Link_KorisniciUlogi]
	(Korisnici_HK, Ulogi_HK)
WHERE Korisnici_HK IS NOT NULL AND Ulogi_HK IS NOT NULL
