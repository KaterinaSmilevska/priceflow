CREATE NONCLUSTERED INDEX [IX_Sat_DnevenPromet]
	ON [dbo].[Sat_DnevenPromet]
	(HartiiOdVrednost_HK, Datum)
WHERE HartiiOdVrednost_HK IS NOT NULL AND Datum IS NOT NULL
