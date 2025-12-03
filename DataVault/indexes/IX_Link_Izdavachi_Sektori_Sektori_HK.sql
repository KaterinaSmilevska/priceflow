CREATE NONCLUSTERED INDEX [IX_Link_Izdavachi_Sektori_Sektori_HK]
	ON [dbo].[Link_Izdavachi_Sektori]
	(Sektori_HK)
WHERE Sektori_HK IS NOT NULL
