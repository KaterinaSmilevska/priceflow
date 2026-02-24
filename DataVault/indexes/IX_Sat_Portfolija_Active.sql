CREATE NONCLUSTERED INDEX [IX_Sat_Portfolija_Active]
	ON [dbo].[Sat_Portfolija]
	(Portfolija_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
