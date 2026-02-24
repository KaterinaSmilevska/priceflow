CREATE NONCLUSTERED INDEX [IX_Sat_HartiiodVrednost_Active]
	ON [dbo].[Sat_HartiiOdVrednost]
	(HartiiOdVrednost_HK, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
