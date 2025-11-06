CREATE NONCLUSTERED INDEX [UX_PortfolioPrinosi_PortfolioId]
	ON [dbo].[PortfolioPrinosi]
	(PortfolioId)
WHERE PortfolioId IS NOT NULL
