CREATE NONCLUSTERED INDEX [IX_Sat_DnevenPromet_Active]
	ON [dbo].[Sat_DnevenPromet]
	(HartiiOdVrednost_HK, Datum, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
