CREATE NONCLUSTERED INDEX [IX_Sat_Izdavachi_HashDiff]
	ON [dbo].[Sat_Izdavachi]
	(HashDiff)
WHERE HashDiff IS NOT NULL
