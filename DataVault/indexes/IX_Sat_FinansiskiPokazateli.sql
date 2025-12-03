CREATE NONCLUSTERED INDEX [IX_Sat_FinansiskiPokazateli]
	ON [dbo].[Sat_FinansiskiPokazateli]
	(Izdavachi_HK, Godina)
WHERE Izdavachi_HK IS NOT NULL AND Godina IS NOT NULL
