CREATE UNIQUE NONCLUSTERED INDEX [UX_Hub_Portfolija_Ime]
	ON [dbo].[Hub_Portfolija]
	(Ime)
WHERE Ime IS NOT NULL
