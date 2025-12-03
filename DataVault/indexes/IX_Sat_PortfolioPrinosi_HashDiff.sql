CREATE NONCLUSTERED INDEX [IX_Sat_PortfolioPrinosi_HashDiff]
	ON [dbo].[Sat_PortfolioPrinosi]
	(HashDiff)
WHERE HashDiff IS NOT NULL
