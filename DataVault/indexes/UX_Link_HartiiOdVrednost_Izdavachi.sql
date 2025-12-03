CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_HartiiOdVrednost_Izdavachi]
	ON [dbo].[Link_HartiiOdVrednost_Izdavachi]
	(HartiiOdVrednost_HK, Izdavachi_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL AND Izdavachi_HK IS NOT NULL
