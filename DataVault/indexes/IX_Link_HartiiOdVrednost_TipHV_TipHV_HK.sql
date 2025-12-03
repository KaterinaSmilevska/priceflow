CREATE INDEX [IX_Link_HartiiOdVrednost_TipHV_TipHV_HK]
	ON [dbo].[Link_HartiiOdVrednost_TipHV]
	(TipHV_HK)
WHERE TipHV_HK IS NOT NULL
