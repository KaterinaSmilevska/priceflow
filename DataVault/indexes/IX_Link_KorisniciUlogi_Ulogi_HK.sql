CREATE NONCLUSTERED INDEX [IX_Link_KorisniciUlogi_Ulogi_HK]
	ON [dbo].[Link_KorisniciUlogi]
	(Ulogi_HK)
WHERE Ulogi_HK IS NOT NULL
