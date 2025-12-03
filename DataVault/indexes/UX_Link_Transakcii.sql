CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_Transakcii]
	ON [dbo].[Link_Transakcii]
	(HartiiOdVrednost_HK, Portfolija_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL AND Portfolija_HK IS NOT NULL
