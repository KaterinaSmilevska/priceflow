CREATE UNIQUE NONCLUSTERED INDEX [UX_Portfolija_Ime]
	ON [dbo].[Portfolija]
	(Ime)
WHERE Ime IS NOT NULL
