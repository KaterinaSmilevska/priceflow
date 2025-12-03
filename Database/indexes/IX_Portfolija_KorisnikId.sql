CREATE NONCLUSTERED INDEX [IX_Portfolija_KorisnikId]
	ON [dbo].[Portfolija]
	(KorisnikId)
WHERE KorisnikId IS NOT NULL
