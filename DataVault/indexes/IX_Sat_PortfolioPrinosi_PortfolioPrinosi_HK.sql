CREATE NONCLUSTERED INDEX [IX_Sat_PortfolioPrinosi_PortfolioPrinosi_HK]
	ON [dbo].[Sat_PortfolioPrinosi]
	(PortfolioPrinosi_HK)
WHERE PortfolioPrinosi_HK IS NOT NULL
