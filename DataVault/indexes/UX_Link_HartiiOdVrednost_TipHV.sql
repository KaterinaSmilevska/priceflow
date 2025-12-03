CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_HartiiOdVrednost_TipHV]
	ON [dbo].[Link_HartiiOdVrednost_TipHV]
	(HartiiOdVrednost_HK, TipHV_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL AND TipHV_HK IS NOT NULL
