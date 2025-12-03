CREATE NONCLUSTERED INDEX [IX_Link_PortfolioPrinosi_HartiiOdVrednost_HK]
	ON [dbo].[Link_PortfolioPrinosi]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
