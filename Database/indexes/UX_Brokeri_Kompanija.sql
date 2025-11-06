CREATE UNIQUE NONCLUSTERED INDEX [UX_Brokeri_Kompanija]
	ON [dbo].[Brokeri]
	(Kompanija)
WHERE Kompanija IS NOT NULL
