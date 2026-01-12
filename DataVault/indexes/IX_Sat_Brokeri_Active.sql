CREATE NONCLUSTERED INDEX [IX_Sat_Brokeri_Active]
	ON [dbo].[Sat_Brokeri]
	(Brokeri_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
