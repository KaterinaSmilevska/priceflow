CREATE NONCLUSTERED INDEX [IX_Sat_Korisnici_Sensitive_Korisnici_HK]
	ON [dbo].[Sat_Korisnici]
	(Korisnici_HK)
WHERE Korisnici_HK IS NOT NULL
