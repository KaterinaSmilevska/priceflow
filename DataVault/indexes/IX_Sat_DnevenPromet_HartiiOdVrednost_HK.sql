CREATE NONCLUSTERED INDEX [IX_Sat_DnevenPromet_HartiiOdVrednost_HK]
	ON [dbo].[Sat_DnevenPromet]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
