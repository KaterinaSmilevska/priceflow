CREATE UNIQUE NONCLUSTERED INDEX [UX_Link_Izdavachi_Sektori]
	ON [dbo].[Link_Izdavachi_Sektori]
	(Izdavachi_HK, Sektori_HK)
WHERE Izdavachi_HK IS NOT NULL AND Sektori_HK IS NOT NULL
