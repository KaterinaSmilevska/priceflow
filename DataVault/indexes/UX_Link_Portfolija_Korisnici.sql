CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_Portfolija_Korisnici]
	ON [dbo].[Link_Portfolija_Korisnici]
	(Portfolija_HK, Korisnici_HK)
WHERE Portfolija_HK IS NOT NULL AND Korisnici_HK IS NOT NULL
