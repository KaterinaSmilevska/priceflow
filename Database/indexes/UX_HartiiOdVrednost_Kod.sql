CREATE UNIQUE NONCLUSTERED INDEX [UX_HartiiOdVrednost_Kod]
	ON [dbo].[HartiiOdVrednost]
	(Kod)
WHERE Kod IS NOT NULL
