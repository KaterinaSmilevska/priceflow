CREATE NONCLUSTERED INDEX [IX_Izdavachi_SektorId]
	ON [dbo].[Izdavachi]
	(SektorId)
WHERE SektorId IS NOT NULL
