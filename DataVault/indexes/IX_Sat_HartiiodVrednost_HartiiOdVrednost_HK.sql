CREATE NONCLUSTERED INDEX [IX_Sat_HartiiodVrednost_HartiiOdVrednost_HK]
	ON [dbo].[Sat_HartiiOdVrednost]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
