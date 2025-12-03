CREATE NONCLUSTERED INDEX [IX_Transakcii_PortfolioId]
	ON [dbo].[Transakcii]
	(PortfolioId)
WHERE PortfolioId IS NOT NULL
