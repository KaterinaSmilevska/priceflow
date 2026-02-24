CREATE NONCLUSTERED INDEX [IX_Sat_PortfolioPrinosi]
	ON [dbo].[Sat_PortfolioPrinosi]
	(PortfolioPrinosi_HK, Datum, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
