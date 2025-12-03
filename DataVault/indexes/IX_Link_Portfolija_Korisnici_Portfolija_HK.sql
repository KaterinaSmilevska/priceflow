CREATE NONCLUSTERED INDEX [IX_Link_Portfolija_Korisnici_Portfolija_HK]
	ON [dbo].[Link_Portfolija_Korisnici]
	(Portfolija_HK)
WHERE Portfolija_HK IS NOT NULL