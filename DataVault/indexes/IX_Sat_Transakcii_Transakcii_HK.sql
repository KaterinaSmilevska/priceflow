CREATE NONCLUSTERED INDEX [IX_Sat_Transakcii_Transakcii_HK]
	ON [dbo].[Sat_Transakcii]
	(Transakcii_HK)
WHERE Transakcii_HK IS NOT NULL
