CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_PortfolioPrinosi]
	ON [dbo].[Link_PortfolioPrinosi]
	(Portfolija_HK, HartiiOdVrednost_HK)
WHERE Portfolija_HK IS NOT NULL AND HartiiOdVrednost_HK IS NOT NULL
