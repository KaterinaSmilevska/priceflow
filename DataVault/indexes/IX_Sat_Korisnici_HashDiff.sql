CREATE NONCLUSTERED INDEX [IX_Sat_Korisnici_HashDiff]
	ON [dbo].[Sat_Korisnici]
	(HashDiff)
WHERE HashDiff IS NOT NULL
