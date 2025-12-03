CREATE NONCLUSTERED INDEX [IX_HartiiOdVrednost_TipHVId]
	ON [dbo].[HartiiOdVrednost]
	(TipHVId)
WHERE TipHVId IS NOT NULL
