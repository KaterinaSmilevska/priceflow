CREATE NONCLUSTERED INDEX [IX_Link_HartiiOdVrednost_Izdavachi_HartiiOdVrednost_HK]
	ON [dbo].[Link_HartiiOdVrednost_Izdavachi]
	(HartiiOdVrednost_HK)
WHERE HartiiOdVrednost_HK IS NOT NULL
