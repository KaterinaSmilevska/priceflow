CREATE NONCLUSTERED INDEX [IX_Sat_Brokeri_Brokeri_HK]
	ON [dbo].[Sat_Brokeri]
	(Brokeri_HK)
WHERE Brokeri_HK IS NOT NULL
