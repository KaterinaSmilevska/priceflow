CREATE NONCLUSTERED INDEX [IX_KorisniciUlogi_UlogaId]
	ON [dbo].[KorisniciUlogi]
	(UlogaId)
WHERE UlogaId IS NOT NULL
