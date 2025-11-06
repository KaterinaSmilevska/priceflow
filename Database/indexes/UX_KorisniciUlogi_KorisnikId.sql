CREATE NONCLUSTERED INDEX [UX_KorisniciUlogi_KorisnikId]
	ON [dbo].[KorisniciUlogi]
	(KorisnikId)
WHERE KorisnikId IS NOT NULL
