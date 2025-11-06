CREATE NONCLUSTERED INDEX [UX_Izdavachi_SektorId]
	ON [dbo].[Izdavachi]
	(SektorId)
WHERE SektorId IS NOT NULL
