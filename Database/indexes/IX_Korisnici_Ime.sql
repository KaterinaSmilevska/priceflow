CREATE NONCLUSTERED INDEX [IX_Korisnici_Ime]
	ON [dbo].[Korisnici]
	(Ime)
WHERE Ime IS NOT NULL