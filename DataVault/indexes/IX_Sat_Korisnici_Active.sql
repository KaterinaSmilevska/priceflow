CREATE NONCLUSTERED INDEX [IX_Sat_Korisnici_Active]
	ON [dbo].[Sat_Korisnici]
	(Korisnici_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
