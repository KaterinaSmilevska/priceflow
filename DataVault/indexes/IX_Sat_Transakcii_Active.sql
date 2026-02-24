CREATE NONCLUSTERED INDEX [IX_Sat_Transakcii_Active]
	ON [dbo].[Sat_Transakcii]
	(Transakcii_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
