CREATE NONCLUSTERED INDEX [IX_KorisniciUlogi_KorisnikId]
	ON [dbo].[KorisniciUlogi]
	(KorisnikId)
WHERE KorisnikId IS NOT NULL
