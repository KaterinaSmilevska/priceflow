CREATE NONCLUSTERED INDEX [IX_Sat_Portfolija_HashDiff]
	ON [dbo].[Sat_Portfolija]
	(HashDiff)
WHERE HashDiff IS NOT NULL
