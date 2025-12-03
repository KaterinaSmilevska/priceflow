CREATE NONCLUSTERED INDEX [IX_Sat_FinansiskiPokazateli_Izdavachi_HK]
	ON [dbo].[Sat_FinansiskiPokazateli]
	(Izdavachi_HK)
WHERE Izdavachi_HK IS NOT NULL
