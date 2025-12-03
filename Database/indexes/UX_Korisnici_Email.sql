CREATE UNIQUE NONCLUSTERED INDEX [UX_Korisnici_Email]
	ON [dbo].[Korisnici]
	(Email)
WHERE Email IS NOT NULL
