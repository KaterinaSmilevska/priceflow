CREATE NONCLUSTERED INDEX [IX_Sat_Izdavachi_Active]
	ON [dbo].[Sat_Izdavachi]
	(Izdavachi_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
