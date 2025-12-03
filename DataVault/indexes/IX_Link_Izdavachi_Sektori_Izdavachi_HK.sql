CREATE NONCLUSTERED INDEX [IX_Link_Izdavachi_Sektori_Izdavachi_HK]
	ON [dbo].[Link_Izdavachi_Sektori]
	(Izdavachi_HK)
WHERE Izdavachi_HK IS NOT NULL
