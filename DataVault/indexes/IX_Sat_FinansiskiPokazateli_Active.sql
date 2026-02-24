CREATE NONCLUSTERED INDEX [IX_Sat_FinansiskiPokazateli_Active]
	ON [dbo].[Sat_FinansiskiPokazateli]
	(Izdavachi_HK, Godina, HashDiff)
	INCLUDE (LoadDate)
WHERE EndDate IS NULL
