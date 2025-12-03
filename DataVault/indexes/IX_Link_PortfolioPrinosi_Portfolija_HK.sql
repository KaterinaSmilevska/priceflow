CREATE NONCLUSTERED INDEX [IX_Link_PortfolioPrinosi_Portfolija_HK]
	ON [dbo].[Link_PortfolioPrinosi]
	(Portfolija_HK)
WHERE Portfolija_HK IS NOT NULL
