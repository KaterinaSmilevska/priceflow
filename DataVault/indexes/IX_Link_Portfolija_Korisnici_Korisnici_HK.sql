CREATE NONCLUSTERED INDEX [IX_Link_Portfolija_Korisnici_Korisnici_HK]
	ON [dbo].[Link_Portfolija_Korisnici]
	(Korisnici_HK)
WHERE Korisnici_HK IS NOT NULL
