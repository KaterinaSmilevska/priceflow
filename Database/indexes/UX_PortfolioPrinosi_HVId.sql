CREATE NONCLUSTERED INDEX [UX_PortfolioPrinosi_HVId]
	ON [dbo].[PortfolioPrinosi]
	(HVId)
WHERE HVId IS NOT NULL
