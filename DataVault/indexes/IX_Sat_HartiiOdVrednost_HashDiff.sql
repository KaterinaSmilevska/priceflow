CREATE NONCLUSTERED INDEX [IX_Sat_HartiiOdVrednost_HashDiff]
	ON [dbo].[Sat_HartiiOdVrednost]
	(HashDiff)
WHERE HashDiff IS NOT NULL
