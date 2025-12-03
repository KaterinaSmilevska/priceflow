CREATE NONCLUSTERED INDEX [IX_Sat_Transakcii_HashDiff]
	ON [dbo].[Sat_Transakcii]
	(HashDiff)
WHERE HashDiff IS NOT NULL