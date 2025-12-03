CREATE INDEX [IX_Link_HartiiOdVrednost_Izdavachi_Izdavachi_HK]
	ON [dbo].[Link_HartiiOdVrednost_Izdavachi]
	(Izdavachi_HK)
WHERE Izdavachi_HK IS NOT NULL
