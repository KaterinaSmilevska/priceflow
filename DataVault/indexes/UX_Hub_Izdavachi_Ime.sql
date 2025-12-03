CREATE UNIQUE NONCLUSTERED INDEX [UX_Hub_Izdavachi_Ime]
	ON [dbo].[Hub_Izdavachi]
	(Ime)
WHERE Ime IS NOT NULL
