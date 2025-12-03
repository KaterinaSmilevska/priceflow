CREATE UNIQUE NONCLUSTERED INDEX [UX_Hub_HartiiOdVrednost_Kod]
	ON [dbo].[Hub_HartiiOdVrednost]
	(Kod)
WHERE Kod IS NOT NULL
