CREATE NONCLUSTERED INDEX [IX_Sat_Izdavachi_Izdavachi_HK]
	ON [dbo].[Sat_Izdavachi]
	(Izdavachi_HK)
WHERE Izdavachi_HK IS NOT NULL
