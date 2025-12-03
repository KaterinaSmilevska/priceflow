CREATE NONCLUSTERED INDEX [IX_Sat_FinansiskiPokazateli_HashDiff]
	ON [dbo].[Sat_FinansiskiPokazateli]
	(HashDiff)
WHERE HashDiff IS NOT NULL
