CREATE NONCLUSTERED INDEX [IX_Link_Transakcii_Portfolija_HK]
	ON [dbo].[Link_Transakcii]
	(Portfolija_HK)
WHERE Portfolija_HK IS NOT NULL
