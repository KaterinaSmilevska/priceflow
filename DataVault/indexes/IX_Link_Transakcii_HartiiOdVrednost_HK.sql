CREATE NONCLUSTERED INDEX [IX_Link_Transakcii_HartiiOdVrednost_HK]
	ON [dbo].[Link_Transakcii]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
