CREATE NONCLUSTERED INDEX [IX_Sat_Portfolija_Portfolija_HK]
	ON [dbo].[Sat_Portfolija]
	(Portfolija_HK)
WHERE Portfolija_HK IS NOT NULL
