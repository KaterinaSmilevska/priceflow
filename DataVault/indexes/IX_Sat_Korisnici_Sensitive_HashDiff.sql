CREATE NONCLUSTERED INDEX [IX_Sat_Korisnici_Sensitive_HashDiff]
	ON [dbo].[Sat_Korisnici]
	(HashDiff)
WHERE HashDiff IS NOT NULL
