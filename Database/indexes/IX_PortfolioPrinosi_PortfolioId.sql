CREATE NONCLUSTERED INDEX [IX_PortfolioPrinosi_PortfolioId]
	ON [dbo].[PortfolioPrinosi]
	(PortfolioId)
WHERE PortfolioId IS NOT NULL
