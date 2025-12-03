CREATE NONCLUSTERED INDEX [IX_Sat_PortfolioPrinosi]
	ON [dbo].[Sat_PortfolioPrinosi]
	(PortfolioPrinosi_HK, Datum)
WHERE PortfolioPrinosi_HK IS NOT NULL AND Datum IS NOT NULL
