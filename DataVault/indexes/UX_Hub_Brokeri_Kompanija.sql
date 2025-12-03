CREATE UNIQUE NONCLUSTERED INDEX UX_Hub_Brokeri_Kompanija
ON [dbo].[Hub_Brokeri]
	(Kompanija)
WHERE Kompanija IS NOT NULL