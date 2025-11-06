CREATE UNIQUE NONCLUSTERED INDEX [UX_Korisnici_Username]
	ON [dbo].[Korisnici]
	(Username)
WHERE Username IS NOT NULL
