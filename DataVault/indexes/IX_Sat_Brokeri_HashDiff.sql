CREATE NONCLUSTERED INDEX [IX_Sat_Brokeri_HashDiff]
	ON [dbo].[Sat_Brokeri]
	(HashDiff)
WHERE HashDiff IS NOT NULL
