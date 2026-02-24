CREATE NONCLUSTERED INDEX [IX_Izvestuvanja_PromenaCena_Procitano]
	ON [dbo].[Izvestuvanja_PromenaCena]
	(KorisnikId, Procitano)
INCLUDE (DatumTrguvanje, ProcentPromena, HVId)