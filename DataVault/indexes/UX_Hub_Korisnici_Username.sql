CREATE UNIQUE NONCLUSTERED INDEX [UX_Hub_Korisnici_Username]
	ON [dbo].[Hub_Korisnici]
	(Username)
WHERE Username IS NOT NULL
