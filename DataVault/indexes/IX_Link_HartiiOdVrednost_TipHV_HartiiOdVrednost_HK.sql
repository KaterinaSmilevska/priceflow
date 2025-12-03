CREATE NONCLUSTERED INDEX [IX_Link_HartiiOdVrednost_TipHV_HartiiOdVrednost_HK]
	ON [dbo].[Link_HartiiOdVrednost_TipHV]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
