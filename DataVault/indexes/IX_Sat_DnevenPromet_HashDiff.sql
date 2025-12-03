CREATE NONCLUSTERED INDEX [IX_Sat_DnevenPromet_HashDiff]
	ON [dbo].[Sat_DnevenPromet]
	(HashDiff)
WHERE HashDiff IS NOT NULL
